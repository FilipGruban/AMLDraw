using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class Polygon
{
    public List<Point> points { get; set; }
    public Color color { get; set; }
    public double lineWidth { get; set; }

    public Polygon(double lineWidth, Color color)
    {
        points = new List<Point>();
        this.lineWidth = lineWidth;
        this.color = color;   
    }

    public void addPoint(Point point)
    {
        points.Add(point);   
    }
}