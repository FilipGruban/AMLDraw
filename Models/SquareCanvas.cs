using System.Collections.Generic;

namespace AvaloniaApplication3.Models;

public class SquareCanvas
{
    private List<Square> _polygons;

    public SquareCanvas()
    {
        _polygons = new List<Square>();
    }
    
    public void addSquare(Square polygon)
    {
        _polygons.Add(polygon);
    }
    public void clear()
    {
        _polygons = new List<Square>();
    }
    public List<Square> GetSquares()
    {
        return _polygons;
    }
}