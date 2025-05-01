using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;
using Line = AvaloniaApplication3.Models.Line;

namespace AvaloniaApplication3.Rasterizers;

public class DashedLineRasterizer : ILineRasterizer
{
    
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    private DeletedAreaCanvas deletedAreaCanvas;
    public DashedLineRasterizer(VeryCoolWriteableBitmap bitmap, Canvas previewCanvas, DeletedAreaCanvas deletedAreaCanvas)
    {
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
        this.deletedAreaCanvas = deletedAreaCanvas;
    }
    private IEnumerable<(int x, int y)> GetDashedLinePixels(DashedLine line)
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

        double dashSize = line.dash;
        double gapSize = line.gap + line.LineWidth;
        double segmentLength = 0;
        bool drawing = true;

        int radius = (int)Math.Floor(line.LineWidth / 2);

        for (int i = 0; i <= steps; i++)
        {
            if (drawing)
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
            }

            segmentLength += 1;

            if (drawing && segmentLength >= dashSize)
            {
                drawing = false;
                segmentLength = 0;
            }
            else if (!drawing && segmentLength >= gapSize)
            {
                drawing = true;
                segmentLength = 0;
            }

            x += xInc;
            y += yInc;
        }
    }
    public void Rasterize(Line line)
    {
        if (line is not DashedLine dashedLine)
            return;
        
        foreach (var (x, y) in GetDashedLinePixels(dashedLine))
        {
            bitmap.SetPixel(x, y, line.LineColor);
        }
    }
    public void RasterizePreviewToCanvas(Line line)
    {
        if (line is not DashedLine dashedLine)
            return;
        
        previewCanvas.Children.Clear();
        
        foreach (var (x, y) in GetDashedLinePixels(dashedLine))
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
        if (line is not DashedLine dashedLine)
            return;
        
        foreach (var (x, y) in GetDashedLinePixels(dashedLine))
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