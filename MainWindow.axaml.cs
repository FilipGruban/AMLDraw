using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication3.models;
using AvaloniaApplication3.Models;
using AvaloniaApplication3.Rasterizers;
using Point = AvaloniaApplication3.Models.Point;

namespace AvaloniaApplication3;

public partial class MainWindow : Window
{
    private Point _point;
    private bool _isSolid = true;
    private double _thicknes = 1;
    private Color _color = Colors.Black;
    private Canvas _canvas;
    private VeryCoolWriteableBitmap _bitmap;
    private Canvas _previewCanvas;
    private Image _image;
    private Slider _widthSlider;
    private ColorPicker _colorPicker;

    private LineCanvas _lineCanvas;
    private LineRasterizer _lineRasterizer;

    private DashedLineCanvas _dashedLineCanvas;
    private DashedLineRasterizer _dashedLineRasterizer;
    private bool _isDashed = false;
    private double _dashedLineGap = 5;
    private double _dashSize = 10;

    private DottedLineRasterizer _dottedLineRasterizer;
    private DottedLineCanvas _dottedLineCanvas;
    private bool _isDotted = false;
    private double _dottedLineGap = 5;
    private Shapes _selectedShape = Shapes.None;

    private PolygonCanvas _polygonCanvas;
    private PolygonRasterizer _polygonRasterizer;
    private Polygon _polygon;

    private Square _square;
    private SquareRasterizer _squareRasterizer;
    private SquareCanvas _squareCanvas;
    
    private CoolRectangle _rectangle;
    private CoolRectangleCanvas _rectangleCanvas;
    private RectangleRasterizer _rectangleRasterizer;
    
    private Circle _circle;
    private CircleCanvas _circleCanvas;
    private CircleRasterizer _circleRasterizer;

    private bool _bucket = false;
    private BucketRasterizer _bucketRasterizer;
    private FilledAreaCanvas _filledAreaCanvas;
    
    private bool _eraser = false;
    private DeletedAreaCanvas _deletedAreaCanvas;
    private EraserRasterizer _eraserRasterizer;
    
    private enum Shapes
    {
        None,
        Square,
        Rectangle,
        Circle,
        Polygon
    }

    public MainWindow()
    {
        InitializeComponent();
        this.KeyDown += OnKeyDown;
        _widthSlider = this.FindControl<Slider>("widthSlider");
        _widthSlider.ValueChanged += (sender, args) => { _thicknes = _widthSlider.Value; };
        _colorPicker = this.FindControl<ColorPicker>("ColorPicker");
        _colorPicker.Color = _color;
        _colorPicker.ColorChanged += (sender, args) =>
        {
            _colorPicker.Color = args.NewColor;
            _color = _colorPicker.Color;
        };

        _canvas = this.FindControl<Canvas>("Canvas");
        _previewCanvas = this.FindControl<Canvas>("PreviewCanvas");
        _deletedAreaCanvas = new DeletedAreaCanvas();

        _bitmap = new VeryCoolWriteableBitmap(_deletedAreaCanvas,new PixelSize(1920, 1080), new Vector(96, 96), PixelFormat.Bgra8888,
            AlphaFormat.Premul);
        _image = new Image { Source = _bitmap.GetWriteableBitmap() };
        _canvas.Children.Add(_image);

        _lineCanvas = new LineCanvas();
        _lineRasterizer = new LineRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);
        _dashedLineRasterizer = new DashedLineRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);
        _dashedLineCanvas = new DashedLineCanvas();
        _dottedLineRasterizer = new DottedLineRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);
        _dottedLineCanvas = new DottedLineCanvas();

        _polygonCanvas = new PolygonCanvas();
        _polygonRasterizer = new PolygonRasterizer(_bitmap, _deletedAreaCanvas);
        
        _squareCanvas = new SquareCanvas();
        _squareRasterizer = new SquareRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);
        
        _rectangleCanvas = new CoolRectangleCanvas();
        _rectangleRasterizer = new RectangleRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);
        
        _circleCanvas = new CircleCanvas();
        _circleRasterizer = new CircleRasterizer(_bitmap, _previewCanvas, _deletedAreaCanvas);;
        _filledAreaCanvas = new FilledAreaCanvas();
        _bucketRasterizer = new BucketRasterizer(_bitmap, _filledAreaCanvas, _deletedAreaCanvas);
        
        _eraserRasterizer = new EraserRasterizer(_deletedAreaCanvas, _bitmap, _previewCanvas);;
        
        this.Title = "Very cool title UwU";
    }

    //vytvoreni cary podle typu 
    private Line createLine(Point endingPoint)
    {
        Line line;
        if (_isDashed)
        {
            line = new DashedLine(_point, endingPoint, _color, _thicknes, _dashedLineGap, _dashSize);
            return line;
        }

        if (_isDotted)
        {
            line = new DottedLine(_point, endingPoint, _color, _thicknes, _dottedLineGap + _thicknes);
            return line;
        }
        else
        {
            line = new Line(_point, endingPoint, _color, _thicknes);
            return line;
        }

    }
    
    //vykresleni cary podle typu 
    private void RasterizePreview(Line line)
    {
        switch (line)
        {
            case DashedLine dashed:
                _dashedLineRasterizer.RasterizePreviewToCanvas(dashed);
                break;
            case DottedLine dotted:
                _dottedLineRasterizer.RasterizePreviewToCanvas(dotted);
                break;
            default:
                _lineRasterizer.RasterizePreviewToCanvas(line);
                break;
        }
    }

    //Ulozeni cary podle typu
    private void AddLineToCanvas(Line line)
    {
        switch (line)
        {
            case DashedLine dashed:
                _dashedLineRasterizer.RasterizeNew(dashed);
                _dashedLineCanvas.addLine(dashed);
                break;
            case DottedLine dotted:
                _dottedLineRasterizer.RasterizeNew(dotted);
                _dottedLineCanvas.addLine(dotted);
                break;
            default:
                _lineRasterizer.RasterizeNew(line);
                _lineCanvas.addLine(line);
                break;
        }
    }

    //vytvoreni bodu podle pozice kurzoru
    private Point getCursorPosition(PointerEventArgs e)
    {
        return new Point(e.GetPosition(_canvas).X, e.GetPosition(_canvas).Y);
    }
    
    //smazani platna
    private void DeleteAll()
    {
        _lineCanvas.clear();
        _dashedLineCanvas.clear();
        _dottedLineCanvas.clear();
        _polygonCanvas.clear();
        _squareCanvas.clear();
        _bitmap.ClearBitmap();
        _image.InvalidateVisual();
        _previewCanvas.Children.Clear();
        _rectangleCanvas.clear();
        _circleCanvas.clear();
        _filledAreaCanvas.Clear();
        _deletedAreaCanvas.Clear();
        _selectedShape = Shapes.None;
    }
    
    //prekresleni platna 
    private void ClearAndRedrawCanvas()
    {
        _previewCanvas.Children.Clear();
        _bitmap.ClearBitmap();
        _bucketRasterizer.Rasterize();
        _lineRasterizer.RasterizeCanvas(_lineCanvas.getLines());
        _dashedLineRasterizer.RasterizeCanvas(_dashedLineCanvas.getLines());
        _dottedLineRasterizer.RasterizeCanvas(_dottedLineCanvas.getLines());
        _polygonRasterizer.RasterizeCanvas(_polygonCanvas.getPolygons(), this._selectedShape == Shapes.Polygon);
        _squareRasterizer.RasterizeCanvas(_squareCanvas.GetSquares());
        _rectangleRasterizer.RasterizeCanvas(_rectangleCanvas.GetRectangles());
        _circleRasterizer.RasterizeCanvas(_circleCanvas.GetCircles());
        _image.InvalidateVisual();
    }

    //zaznamenani kliknuti, provede akci podle vybraneho modu/typu 
    private void MousePressed(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
        {
            if (_eraser)
            {
                _eraserRasterizer.EraseAt((int)getCursorPosition(e).x, (int)getCursorPosition(e).y, (int)_thicknes * 2);
                return;
            }
            if (_bucket) 
            {
                _bucketRasterizer.Fill((int)getCursorPosition(e).x, (int)getCursorPosition(e).y, _color);
                _image.InvalidateVisual();
                return;
            }
            if (_selectedShape == Shapes.Polygon)
            {
                _polygon.addPoint(getCursorPosition(e));
            }
            else if(_selectedShape == Shapes.Square)
            {
                _point = getCursorPosition(e);
                _square = new Square(_point, getCursorPosition(e), _color, _thicknes);
            }
            else if (_selectedShape == Shapes.Rectangle)
            {
                _point = getCursorPosition(e);
                _rectangle = new CoolRectangle(_point, getCursorPosition(e), _color, _thicknes);
            }
            else if (_selectedShape == Shapes.Circle)
            {
                _point = getCursorPosition(e);
                _circle = new Circle(_point, getCursorPosition(e), _color, _thicknes);
            }
            else if (_selectedShape == Shapes.None)
            {
                _point = getCursorPosition(e);
            }
        }
    }

    //zaznamenani posouvani kurzoru
    private void MouseDragged(object sender, PointerEventArgs e)
    {
        if (_bucket)
        {
            return;
        }
        if (_selectedShape == Shapes.Polygon)
        {
            return;
        }
        
        if (e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
        {
            if (_eraser)
            {
                _eraserRasterizer.EraseAt((int)getCursorPosition(e).x, (int)getCursorPosition(e).y, (int)_thicknes * 2);
                return;
            }
            if (_selectedShape == Shapes.Square)
            {
                _square.endPoint = getCursorPosition(e);
                _squareRasterizer.RasterizePreviewToCanvas(_square);
            }
            else if (_selectedShape == Shapes.Rectangle)
            {
                _rectangle.endPoint = getCursorPosition(e);
                _rectangleRasterizer.RasterizePreviewToCanvas(_rectangle);
            }
            else if (_selectedShape == Shapes.Circle)
            {
                _circle.endPoint = getCursorPosition(e);
                _circleRasterizer.RasterizePreviewToCanvas(_circle);
            }
            else if (_selectedShape == Shapes.None)
            {
                var newLine = createLine(getCursorPosition(e));
                RasterizePreview(newLine);    
            }
                            
        }
    }
    
    //zaznamenani pusteni leveho tlacitka 
    private void MouseReleased(object sender, PointerReleasedEventArgs e)
    {
        if (_eraser)
        {
            ClearAndRedrawCanvas();
            return;
        }
        if (_bucket)
        {
            _bucket = false;
            ClearAndRedrawCanvas();
            return;
        }
        if (_selectedShape == Shapes.Square)
        {
            _squareRasterizer.RasterizeNew(_square);
            _squareCanvas.addSquare(_square);
            _selectedShape = Shapes.None;
        }
        else if (_selectedShape == Shapes.Rectangle)
        {
            _rectangleRasterizer.RasterizeNew(_rectangle);
            _rectangleCanvas.addRectangle(_rectangle);
            _selectedShape = Shapes.None;
        }
        else if (_selectedShape == Shapes.Circle)
        {
            _circleRasterizer.RasterizeNew(_circle);
            _circleCanvas.addCircle(_circle);
            _selectedShape = Shapes.None;
        }
        else if (_selectedShape == Shapes.None)
        {
            var newLine = createLine(getCursorPosition(e));

            AddLineToCanvas(newLine);

        }
        _previewCanvas.Children.Clear();
        ClearAndRedrawCanvas();
    }
    
    //zaznamenani stisknuti klavesy
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        _selectedShape = Shapes.None;

        if (e.Key == Key.C)
        {
            DeleteAll();
        }
        else if (e.Key == Key.LeftCtrl)
        {
            changeLineType("dashed");
        }
        else if (e.Key == Key.LeftAlt)
        {
            changeLineType("dotted");
        }
        else
        {
            changeLineType("solid");
        }
    }

    //pomocna funkce pro zmena typu cary
    private void changeLineType(string type)
    {
        _isDotted = false;
        _isDashed = false;
        _bucket = false;
        _eraser = false;
        switch (type)
        {
            case "dashed":
                _isDashed = true;
                break;
            case "dotted":
                _isDotted = true;
                break;
            case "solid":
                _isSolid = true;
                break;
        }
    }
    
    //funkce pro zmenu modu kresleni 
    private void ChangeMode(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
            return;
        _bucket = false;
        _selectedShape = Shapes.None;
        _eraser = false;

        _selectedShape = tag switch
        {
            "square" => Shapes.Square,
            "rectangle" => Shapes.Rectangle,
            "circle" => Shapes.Circle,
            "polygon" => Shapes.Polygon,
            _ => Shapes.None
        };
        if (_selectedShape == Shapes.Polygon)
        {
            _polygon = new Polygon(_thicknes, _color);
            _polygonCanvas.addPolygon(_polygon);
        }
    }
    
    //vybrani kybliku pri kliknuti na tlacitko 
    private void ALlowBucket(object? sender, RoutedEventArgs e)
    {
        _selectedShape = Shapes.None;
        this._eraser = false;
        this._bucket = !_bucket;
    }
    
    //vybrani gumy pri kliknuti na tlacitko
    private void AllowEraser(object? sender, RoutedEventArgs e)
    {
        _selectedShape = Shapes.None;
        this._bucket = false;
        this._eraser = !_eraser;
    }
}