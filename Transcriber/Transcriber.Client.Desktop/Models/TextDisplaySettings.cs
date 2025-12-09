using Avalonia;
using ReactiveUI;

namespace Transcriber.Client.Desktop.Models;

public class TextDisplaySettings : ReactiveObject
{
    private int _windowLeft = 100;
    private int _windowTop = 100;
    private double _windowWidth = 400;
    private double _windowHeight = 200;
    private double _windowOpacity = 0.8;
    private double _textOpacity = 1.0;
    private int _fontSize = 14;

    public int WindowLeft
    {
        get => _windowLeft;
        set 
        {
            this.RaiseAndSetIfChanged(ref _windowLeft, value);
            this.RaisePropertyChanged(nameof(Position));
        }
    }

    public int WindowTop
    {
        get => _windowTop;
        set 
        {
            this.RaiseAndSetIfChanged(ref _windowTop, value);
            this.RaisePropertyChanged(nameof(Position));
        }
    }

    public PixelPoint Position => new PixelPoint(WindowLeft, WindowTop);

    public double WindowWidth
    {
        get => _windowWidth;
        set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
    }

    public double WindowHeight
    {
        get => _windowHeight;
        set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    }

    public double WindowOpacity
    {
        get => _windowOpacity;
        set => this.RaiseAndSetIfChanged(ref _windowOpacity, value);
    }

    public double TextOpacity
    {
        get => _textOpacity;
        set => this.RaiseAndSetIfChanged(ref _textOpacity, value);
    }

    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }
}