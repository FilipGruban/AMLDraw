using System.Collections.Generic;
using AvaloniaApplication3.Models;

namespace AvaloniaApplication3.models;

public class DashedLineCanvas
{
    private List<Line> _lines;

    public DashedLineCanvas()
    {
        _lines = new List<Line>();
    }
    
    public void addLine(Line line)
    {
        _lines.Add(line);
    }

    public void clear()
    {
        _lines = new List<Line>();
    }

    public List<Line> getLines()
    {
        return _lines;
    }
}