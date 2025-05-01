
using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class Line
{
    public Point StartingPoint { get; set; }
    public Point EndingPoint { get; set; }
    public Color LineColor { get; set; }
    public double LineWidth { get; set; }

    public Line(Point startingPoint, Point endingPoint, Color lineColor, double lineWidth)
    {
        this.StartingPoint = startingPoint;
        this.EndingPoint = endingPoint;
        this.LineColor = lineColor;
        this.LineWidth = lineWidth;
    }
    
}