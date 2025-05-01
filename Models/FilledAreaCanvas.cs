using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public class FilledAreaCanvas
{
    //souradnice pixelu a jeho barva 
    private readonly HashSet<(int x, int y, Color color)> _filled = new();

    public void Add(int x, int y, Color color)
    {
        _filled.Add((x, y, color));
    }
    
    public IEnumerable<(int x, int y, Color color)> GetAll()
    {
        return _filled;
    }

    public void Clear()
    {
        _filled.Clear();
    }
}