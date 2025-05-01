using System.Collections.Generic;

namespace AvaloniaApplication3.Models;

public class DeletedAreaCanvas
{
    private readonly HashSet<(int x, int y)> _erased;
    
    public DeletedAreaCanvas()
    {
        this._erased = new HashSet<(int x, int y)>();
    }
    
    public void Add(int x, int y)
    {
        _erased.Add((x, y));
    }
    
    public void Remove(int x, int y)
    {
        _erased.Remove((x, y));
    }

    public bool Contains(int x, int y)
    {
        return _erased.Contains((x, y));
    }

    public void Clear()
    {
        _erased.Clear();
    }

    public IEnumerable<(int x, int y)> GetAll() => _erased;
}