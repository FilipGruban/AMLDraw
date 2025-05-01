using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class Square
{
    public Point startPoint { get; set; }
    public Point endPoint { get; set; }
    public Color lineColor { get; set; }
    public double lineWidth { get; set; }

    public Square(Point startPoint, Point endPoint, Color lineColor, double lineWidth)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.lineColor = lineColor;
        this.lineWidth = lineWidth;
    }
}