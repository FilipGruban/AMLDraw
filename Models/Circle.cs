using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class Circle
{
    public Point startPoint;
    public Point endPoint;
    public Color lineColor;
    public double lineWidth;

    public Circle(Point startPoint, Point endPoint, Color lineColor, double lineWidth)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.lineColor = lineColor;
        this.lineWidth = lineWidth;
    }
}