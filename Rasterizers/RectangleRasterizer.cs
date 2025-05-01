using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;
using Line = AvaloniaApplication3.Models.Line;

namespace AvaloniaApplication3.Rasterizers;

public class RectangleRasterizer
{
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    private LineRasterizer lineRasterizer;
    private DeletedAreaCanvas deletedAreaCanvas;

    public RectangleRasterizer(VeryCoolWriteableBitmap bitmap, Canvas previewCanvas, DeletedAreaCanvas deleted)
    {
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
        this.lineRasterizer = new LineRasterizer(bitmap, previewCanvas, deleted);
        this.deletedAreaCanvas = deleted;
    }

    //pomocna funkce ktera pocita souradnice pixelu k vybarveni
    private IEnumerable<(int x, int y)> GetRectanglePixels(CoolRectangle rectangle)
    {
        var edges = CalculateRectangleEdges(rectangle);

        foreach (var (start, end) in edges)
        {
            foreach (var (x, y) in lineRasterizer.GetLinePixels(new Line(start, end, rectangle.lineColor,
                         rectangle.lineWidth)))
            {
                yield return (x, y);
            }
        }
    }

    private List<(Point, Point)> CalculateRectangleEdges(CoolRectangle rectangle)
    {
        var start = rectangle.startPoint;
        var end = rectangle.endPoint;

        Point p1 = new Point(start.x, start.y);
        Point p2 = new Point(end.x, start.y);
        Point p3 = new Point(end.x, end.y);
        Point p4 = new Point(start.x, end.y);

        return new List<(Point, Point)>
        {
            (p1, p2),
            (p2, p3), 
            (p3, p4), 
            (p4, p1) 
        };
    }
    
    //vykresleni obdelniku
    public void Rasterize(CoolRectangle rectangle)
    {
        foreach (var (x, y) in GetRectanglePixels(rectangle))
        {
            bitmap.SetPixel(x, y, rectangle.lineColor);
        }
    }
    
    //vykresleni noveho obdelniku
    public void RasterizeNew(CoolRectangle rectangle)
    {
        foreach (var (x, y) in GetRectanglePixels(rectangle))
        {
            deletedAreaCanvas.Remove(x,y);
            bitmap.SetPixel(x, y, rectangle.lineColor);
        }
    }
    
    //vykresleni nahledu obdelniku pred vykreslenim na platno
    public void RasterizePreviewToCanvas(CoolRectangle rectangle)
    {
        previewCanvas.Children.Clear();

        foreach (var (x, y) in GetRectanglePixels(rectangle))
        {
            var rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(rectangle.lineColor)
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            previewCanvas.Children.Add(rect);
        }
    }

    //vykresleni vsech obdelniku
    public void RasterizeCanvas(List<CoolRectangle> rectangles)
    {
        foreach (CoolRectangle rectangle in rectangles)
        {
            this.Rasterize(rectangle);
        }
    }
}