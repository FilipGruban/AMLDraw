using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;
using Line = AvaloniaApplication3.Models.Line;

namespace AvaloniaApplication3.Rasterizers;

public class LineRasterizer : ILineRasterizer
{
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    private DeletedAreaCanvas deletedAreaCanvas;

    public IEnumerable<(int x, int y)> GetLinePixels(Line line)
    {
        double x1 = line.StartingPoint.x;
        double y1 = line.StartingPoint.y;
        double x2 = line.EndingPoint.x;
        double y2 = line.EndingPoint.y;

        double dx = x2 - x1;
        double dy = y2 - y1;

        int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));
        if (steps == 0) steps = 1;

        double xInc = dx / steps;
        double yInc = dy / steps;

        double x = x1;
        double y = y1;

        int radius = (int)Math.Floor(line.LineWidth / 2);

        for (int i = 0; i <= steps; i++)
        {
            int centerX = (int)Math.Round(x);
            int centerY = (int)Math.Round(y);

            for (int dxOffset = -radius; dxOffset <= radius; dxOffset++)
            {
                for (int dyOffset = -radius; dyOffset <= radius; dyOffset++)
                {
                    int px = centerX + dxOffset;
                    int py = centerY + dyOffset;

                    if (line.LineWidth == 1 || dxOffset * dxOffset + dyOffset * dyOffset <= radius * radius)
                    {
                        yield return (px, py);
                    }
                }
            }

            x += xInc;
            y += yInc;
        }
    }
    public LineRasterizer(VeryCoolWriteableBitmap bitmap, Canvas previewCanvas, DeletedAreaCanvas deletedAreaCanvas)
    {
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
        this.deletedAreaCanvas = deletedAreaCanvas;
    }
    public void Rasterize(Line line)
    {
        foreach (var (x, y) in GetLinePixels(line))
        {
            bitmap.SetPixel(x, y, line.LineColor);
        }
    }
    public void RasterizePreviewToCanvas(Line line)
    {
        previewCanvas.Children.Clear();
        
        foreach (var (x, y) in GetLinePixels(line))
        {
            var rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(line.LineColor)
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            previewCanvas.Children.Add(rect);
        }
    }

    public void RasterizeNew(Line line)
    {
        foreach (var (x, y) in GetLinePixels(line))
        {
            deletedAreaCanvas.Remove(x,y);
            bitmap.SetPixel(x, y, line.LineColor);
        }
    }

    public void RasterizeCanvas(List<Line> lines)
    {
        foreach (Line line in lines)
        {
            this.Rasterize(line);
        }
    }
}