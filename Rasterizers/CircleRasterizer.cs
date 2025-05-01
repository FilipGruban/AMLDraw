using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;

namespace AvaloniaApplication3.Rasterizers;

public class CircleRasterizer
{
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    private DeletedAreaCanvas deletedAreaCanvas;
    public CircleRasterizer(VeryCoolWriteableBitmap bitmap, Canvas previewCanvas, DeletedAreaCanvas deletedAreaCanvas)
    {
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
        this.deletedAreaCanvas = deletedAreaCanvas;
    }
    //pomocna funkce ktera pocita souradnice kruhu
    private IEnumerable<(int x, int y)> GetCirclePixels(Circle circle)
    {
        double centerX = circle.startPoint.x;
        double centerY = circle.startPoint.y;

        double dx = circle.endPoint.x - centerX;
        double dy = circle.endPoint.y - centerY;
        double radius = Math.Sqrt(dx * dx + dy * dy);

        double thickness = circle.lineWidth;
        int intRadius = (int)Math.Round(radius);
        int intCenterX = (int)Math.Round(centerX);
        int intCenterY = (int)Math.Round(centerY);

        for (int x = -intRadius - 1; x <= intRadius + 1; x++)
        {
            for (int y = -intRadius - 1; y <= intRadius + 1; y++)
            {
                double distance = Math.Sqrt(x * x + y * y);
                if (distance >= radius - thickness / 2 && distance <= radius + thickness / 2)
                {
                    yield return (intCenterX + x, intCenterY + y);
                }
            }
        }
    }
    
    //samotne vybarveni kruhu
    public void Rasterize(Circle circle)
    {
        foreach (var (x, y) in GetCirclePixels(circle))
        {
            bitmap.SetPixel(x, y, circle.lineColor);
        }
    }
    
    //vykresleni noveho kruhu 
    public void RasterizeNew(Circle circle)
    {
        foreach (var (x, y) in GetCirclePixels(circle))
        {
            deletedAreaCanvas.Remove(x,y);
            bitmap.SetPixel(x, y, circle.lineColor);
        }
    }
    
    //vykresleni nahledu kruhu pred vykreslenim na platno
    public void RasterizePreviewToCanvas(Circle circle)
    {
        previewCanvas.Children.Clear();

        foreach (var (x, y) in GetCirclePixels(circle))
        {
            var rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(circle.lineColor)
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            previewCanvas.Children.Add(rect);
        }
    }
    
    //vykresleni vsech kruhu
    public void RasterizeCanvas(List<Circle> circles)
    {
        foreach (Circle circle in circles)
        {
            this.Rasterize(circle);
        }
    }
}