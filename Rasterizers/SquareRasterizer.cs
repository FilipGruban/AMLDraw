using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;
using Line = AvaloniaApplication3.Models.Line;

namespace AvaloniaApplication3.Rasterizers;

public class SquareRasterizer
{
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    private LineRasterizer lineRasterizer;
    private DeletedAreaCanvas deletedAreaCanvas;

    public SquareRasterizer(VeryCoolWriteableBitmap bitmap, Canvas previewCanvas, DeletedAreaCanvas deletedAreaCanvas )
    {
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
        this.lineRasterizer = new LineRasterizer(bitmap, previewCanvas, deletedAreaCanvas);
        this.deletedAreaCanvas = deletedAreaCanvas;
    }
    
    //pomocna funkce ktera pocita souradnice pixelu k vybarveni
    private IEnumerable<(int x, int y)> GetSquarePixels(Square square)
    {
        var edges = CalculateEdges(square);

        foreach (var (start, end) in edges)
        {
            foreach (var (x, y) in lineRasterizer.GetLinePixels(new Line(start, end, square.lineColor, square.lineWidth)))
            {
                yield return (x, y);
            }
        }
    }
    
    //pomocna funkce pomocne funkce, pocita souradni rohu
    private List<(Point, Point)> CalculateEdges(Square square)
    {
        var start = square.startPoint;
        var end = square.endPoint;

        double dx = end.x - start.x;
        double dy = end.y - start.y;
        double side = Math.Min(Math.Abs(dx), Math.Abs(dy));
        double signX = Math.Sign(dx);
        double signY = Math.Sign(dy);

        Point p1 = start;
        Point p2 = new Point(start.x + signX * side, start.y);
        Point p3 = new Point(start.x + signX * side, start.y + signY * side);
        Point p4 = new Point(start.x, start.y + signY * side);

        return new List<(Point, Point)>
        {
            (p1, p2),
            (p2, p3),
            (p3, p4),
            (p4, p1)
        };
    }
    
    //vykresleni ctverce
    public void Rasterize(Square square)
    {
        foreach (var (x, y) in GetSquarePixels(square))
        {
            bitmap.SetPixel(x, y, square.lineColor);
        }
    }
    
    //vykresleni noveho ctverce
    public void RasterizeNew(Square square)
    {
        foreach (var (x, y) in GetSquarePixels(square))
        {
            deletedAreaCanvas.Remove(x,y);
            bitmap.SetPixel(x, y, square.lineColor);
        }
    }

    //vykresleni nahledu ctverce pred vykreslenim na platno
    public void RasterizePreviewToCanvas(Square square)
    {
        previewCanvas.Children.Clear();
        
        foreach (var (x, y) in GetSquarePixels(square))
        {
            var rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(square.lineColor)
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            previewCanvas.Children.Add(rect);
        }
    }

    //vykresleni vsech ctvercu
    public void RasterizeCanvas(List<Square> squares)
    {
        foreach (Square square in squares)
        {
            this.Rasterize(square);
        }
    }
}