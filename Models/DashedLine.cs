using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class DashedLine : Line
{
    public double gap { get; set; }
    public double dash { get; set; }
    public DashedLine(Point startingPoint, Point endingPoint, Color lineColor, double lineWidth,  double gap, double dash): base(startingPoint, endingPoint, lineColor, lineWidth)
    {
        this.gap = gap;
        this.dash = dash;
    }
}