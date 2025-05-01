using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class DottedLine : Line
{
    public double Gap { get; set; }

    public DottedLine(Point startingPoint, Point endingPoint, Color lineColor, double lineWidth, double gap)
        : base(startingPoint, endingPoint, lineColor, lineWidth)
    {
        Gap = gap;
    }
}