using System.Collections.Generic;

namespace AvaloniaApplication3.Models;

public class CircleCanvas
{
    private List<Circle> _rectangles;

    public CircleCanvas()
    {
        _rectangles = new List<Circle>();
    }
    
    public void addCircle(Circle circle)
    {
        _rectangles.Add(circle);
    }
    public void clear()
    {
        _rectangles = new List<Circle>();
    }
    public List<Circle> GetCircles()
    {
        return _rectangles;
    }
}