using System.Collections.Generic;

namespace AvaloniaApplication3.Models;

public class DottedLineCanvas
{

    private List<Line> _lines;

    public DottedLineCanvas()
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