using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;
using Polygon = AvaloniaApplication3.Models.Polygon;

namespace AvaloniaApplication3.Rasterizers;

public class PolygonRasterizer
{
    private VeryCoolWriteableBitmap bitmap;
    private DeletedAreaCanvas deletedArea;
    public PolygonRasterizer(VeryCoolWriteableBitmap bitmap, DeletedAreaCanvas deletedArea)
    {
        this.bitmap = bitmap;
        this.deletedArea = deletedArea;
    }
    
    //pomocna funkce pro pocitani pixelu k vybarveni
    private IEnumerable<(int x, int y)> GetLinePixels(Point start, Point end, int radius)
    {
        int x1 = (int)start.x;
        int y1 = (int)start.y;
        int x2 = (int)end.x;
        int y2 = (int)end.y;

        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = (x1 < x2) ? 1 : -1;
        int sy = (y1 < y2) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            yield return (x1, y1);

            for (int dxOffset = -radius; dxOffset <= radius; dxOffset++)
            {
                for (int dyOffset = -radius; dyOffset <= radius; dyOffset++)
                {
                    if (dxOffset * dxOffset + dyOffset * dyOffset <= radius * radius)
                    {
                        yield return (x1 + dxOffset, y1 + dyOffset);
                    }
                }
            }

            if (x1 == x2 && y1 == y2) break;

            int e2 = err * 2;

            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }

    //vykresleni celeho polygonu
    public void Rasterize(Polygon polygon)
    {
        var points = polygon.points;
        int lineWidth = (int)polygon.lineWidth;
        Color lineColor = polygon.color;

        for (int i = 0; i < points.Count; i++)
        {
            var currentPoint = points[i];
            var nextPoint = points[(i + 1) % points.Count]; 

            foreach (var (x, y) in GetLinePixels(currentPoint, nextPoint, lineWidth / 2))
            {
                
                bitmap.SetPixel(x, y, lineColor);  
            }
        }
    }
    
    //vykresleni noveho polygonu
    public void RasterizeNew(Polygon polygon)
    {
        var points = polygon.points;
        int lineWidth = (int)polygon.lineWidth;
        Color lineColor = polygon.color;

        for (int i = 0; i < points.Count; i++)
        {
            var currentPoint = points[i];
            var nextPoint = points[(i + 1) % points.Count]; 

            foreach (var (x, y) in GetLinePixels(currentPoint, nextPoint, lineWidth / 2))
            {
                deletedArea.Remove(x, y);
                bitmap.SetPixel(x, y, lineColor);  
            }
        }
    }
    
    //vykresleni vsech polygonu, zaroven kontroluje zda je posledni polygon dokoncen
    public void RasterizeCanvas(List<Polygon> polygons, bool NotComplete)
    {
        foreach (Polygon polygon in polygons)
        {
            if (polygon == polygons[^1] && NotComplete)
            {
                RasterizeNew(polygon);
            }
            else
            {
                Rasterize(polygon);
            }
        }
    }
}
