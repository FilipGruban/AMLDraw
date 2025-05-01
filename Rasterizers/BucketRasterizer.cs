using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaApplication3.Models;

namespace AvaloniaApplication3.Rasterizers;

public class BucketRasterizer
{
    private VeryCoolWriteableBitmap _bitmap;
    private FilledAreaCanvas _filledAreaCanvas;
    private DeletedAreaCanvas _deletedAreaCanvas;

    public BucketRasterizer(VeryCoolWriteableBitmap bitmap, FilledAreaCanvas filledAreaCanvas, DeletedAreaCanvas deleted)
    {
        _bitmap = bitmap;
        _filledAreaCanvas = filledAreaCanvas;
        _deletedAreaCanvas = deleted;
    }
    
    //funkce vybarveni pomoci kybliku 
    public void Fill(int startX, int startY, Color fillColor)
    {
        int width = _bitmap.GetWriteableBitmap().PixelSize.Width;
        int height = _bitmap.GetWriteableBitmap().PixelSize.Height;

        if (startX < 0 || startX >= width || startY < 0 || startY >= height)
            return;
        
        Color targetColor = _bitmap.GetPixel(startX, startY);
        if(fillColor == targetColor) return;
        
        Queue<(int x, int y)> queue = new();
        queue.Enqueue((startX, startY));

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            if (_bitmap.GetPixel(x, y) != targetColor)
                continue;

            if (_deletedAreaCanvas.Contains(x, y))
            {
                _deletedAreaCanvas.Remove(x, y);
            }
            _bitmap.SetPixel(x, y, fillColor);
            _filledAreaCanvas.Add(x, y, fillColor);
            
            queue.Enqueue((x + 1, y));
            queue.Enqueue((x - 1, y));
            queue.Enqueue((x, y + 1));
            queue.Enqueue((x, y - 1));
        }
        
    }
    
    public void Rasterize()
    {
        foreach ((int x, int y, Color color) in _filledAreaCanvas.GetAll())
        {
            _bitmap.SetPixel(x, y, color);
        }
    }
}