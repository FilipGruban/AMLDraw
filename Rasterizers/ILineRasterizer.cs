using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaApplication3.Models;

namespace AvaloniaApplication3.Rasterizers;

public interface ILineRasterizer
{
    //vykresleni jedne cary
    void Rasterize(Line line); 
    //vykresleni vsech car
    void RasterizeCanvas(List<Line> lines);
    //vykresleni nahledu cary
    void RasterizePreviewToCanvas(Line line);
    //vykresleni nove cary 
    void RasterizeNew(Line line);
}