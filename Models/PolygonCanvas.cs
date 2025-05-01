using System.Collections.Generic;

namespace AvaloniaApplication3.Models;

public class PolygonCanvas
{
    private List<Polygon> _polygons;

    public PolygonCanvas()
    {
        _polygons = new List<Polygon>();
    }
    
    public void addPolygon(Polygon polygon)
    {
        _polygons.Add(polygon);
    }
    public void clear()
    {
        _polygons = new List<Polygon>();
    }
    public List<Polygon> getPolygons()
    {
        return _polygons;
    }
}