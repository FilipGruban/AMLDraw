using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaApplication3.Models;

namespace AvaloniaApplication3.Rasterizers;

public class EraserRasterizer
{
    private DeletedAreaCanvas deletedArea;
    private VeryCoolWriteableBitmap bitmap;
    private Canvas previewCanvas;
    
    public EraserRasterizer(DeletedAreaCanvas deletedArea, VeryCoolWriteableBitmap bitmap, Canvas previewCanvas)
    {
        this.deletedArea = deletedArea;
        this.bitmap = bitmap;
        this.previewCanvas = previewCanvas;
    }
    
    //smazani oblasti okolo kurzoru
    public void EraseAt(int centerX, int centerY, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int x = centerX + dx;
                int y = centerY + dy;

                if (!IsWithinBounds(x, y)) continue;

                if (Math.Sqrt(dx * dx + dy * dy) <= radius )
                {
                    if (bitmap.GetPixel(x, y) == Color.FromArgb(0, 0, 0, 0)) continue;
                    Rectangle rect = new Rectangle
                    {
                        Width = 1,
                        Height = 1,
                        Fill = new SolidColorBrush(Colors.White)
                    };
                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                    previewCanvas.Children.Add(rect);
                    deletedArea.Add(x, y);
                }
            }
        }
    }

    //funkce kontrolujici jestli je kurzor v kreslici oblasti
    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < bitmap.GetWriteableBitmap().PixelSize.Width &&
               y >= 0 && y < bitmap.GetWriteableBitmap().PixelSize.Height;
    }
}