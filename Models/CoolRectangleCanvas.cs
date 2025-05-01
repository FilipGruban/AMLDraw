using System.Collections.Generic;
using Avalonia;

namespace AvaloniaApplication3.Models;

public class CoolRectangleCanvas
{
    private List<CoolRectangle> _rectangles;

    public CoolRectangleCanvas()
    {
        _rectangles = new List<CoolRectangle>();
    }
    
    public void addRectangle(CoolRectangle rectangle)
    {
        _rectangles.Add(rectangle);
    }
    public void clear()
    {
        _rectangles = new List<CoolRectangle>();
    }
    public List<CoolRectangle> GetRectangles()
    {
        return _rectangles;
    }
}