using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaApplication3.Models;

public class VeryCoolWriteableBitmap
{
    private WriteableBitmap _bitmap;
    private DeletedAreaCanvas _deletedArea;
    
    public VeryCoolWriteableBitmap(DeletedAreaCanvas deletedArea,PixelSize size, Vector dpi, PixelFormat? format = null, AlphaFormat? alphaFormat = null)
    {
        this._bitmap = new WriteableBitmap(size, dpi, format, alphaFormat);
        this._deletedArea = deletedArea;
    }

    public WriteableBitmap GetWriteableBitmap()
    {
        return _bitmap;
    }
    
    //funkce ktera meni barvu jednotliveho pixelu pokud neni oblast vymazana
    public void SetPixel(int x, int y, Color color)
    {
        if (_deletedArea.Contains(x, y))
        {
            return;
        }
        using (var fb = _bitmap.Lock())
        {
            unsafe
            {
                var ptr = (uint*)fb.Address;
                int stride = fb.RowBytes / 4;

                if (x >= 0 && x < _bitmap.PixelSize.Width && y >= 0 && y < _bitmap.PixelSize.Height)
                {
                    uint rgbaColor = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
                    ptr[y * stride + x] = rgbaColor;
                }
            }
        }
    }
    
    //vymazani platna
    public void ClearBitmap()
    {
        using (var fb = _bitmap.Lock())
        {
            unsafe
            {
                var ptr = (uint*)fb.Address;
                int stride = fb.RowBytes / 4;

                for (int y = 0; y < _bitmap.PixelSize.Height; y++)
                {
                    for (int x = 0; x < _bitmap.PixelSize.Width; x++)
                    {
                        ptr[y * stride + x] = 0x00000000;
                    }
                }
            }
        }
    }
    
    //vraci barvu jednotliveho pixelu
    public Color GetPixel(int x, int y)
    {
        using (var fb = _bitmap.Lock())
        {
            unsafe
            {
                var ptr = (uint*)fb.Address;
                int stride = fb.RowBytes / 4;

                if (x >= 0 && x < _bitmap.PixelSize.Width && y >= 0 && y < _bitmap.PixelSize.Height)
                {
                    uint rgbaColor = ptr[y * stride + x];

                    byte r = (byte)((rgbaColor >> 16) & 0xFF);
                    byte g = (byte)((rgbaColor >> 8) & 0xFF);
                    byte b = (byte)(rgbaColor & 0xFF);
                    byte a = (byte)((rgbaColor >> 24) & 0xFF);

                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    return Colors.Transparent;
                }
            }
        }
    }
}
