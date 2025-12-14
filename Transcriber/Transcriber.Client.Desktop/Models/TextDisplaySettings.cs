using System;
using Avalonia;
using ReactiveUI;

namespace Transcriber.Client.Desktop.Models;

public class TextDisplaySettings : ReactiveObject
{
    private int _screenWidth = 400;
    private int _screenHeight = 400;
    private int _windowLeft;
    private int _windowTop;
    private int _windowWidth = 200;
    private int _windowHeight = 100;
    private double _windowOpacity = 0.8;
    private double _textOpacity = 1.0;
    private int _fontSize = 16;

    public int ScreenWidth
    {
        get => _screenWidth;
        set
        {
            var maxLeft = Math.Max(0, _screenWidth - _windowWidth);
            if (_windowLeft > maxLeft)
            {
                WindowLeft = maxLeft;
            }
            else if (_windowLeft < 0)
            {
                WindowLeft = 0;
            }
            this.RaiseAndSetIfChanged(ref _screenWidth, value);
            this.RaisePropertyChanged(nameof(MaxLeft));
        }
    }

    public int ScreenHeight
    {
        get => _screenHeight;
        set
        {
            var maxTop = Math.Max(0, _screenHeight - _windowHeight);
            if (_windowTop > maxTop)
            {
                WindowTop = maxTop;
            }
            else if (_windowTop < 0)
            {
                WindowTop = 0;
            }
            this.RaiseAndSetIfChanged(ref _screenHeight, value);
            this.RaisePropertyChanged(nameof(MaxTop));
        }
    }

    public int WindowLeft
    {
        get => _windowLeft;
        set 
        {
            var maxLeft = Math.Max(0, _screenWidth - _windowWidth);
            var newValue = Math.Clamp(value, 0, maxLeft);

            this.RaiseAndSetIfChanged(ref _windowLeft, newValue);
            this.RaisePropertyChanged(nameof(Position));
        }
    }

    public int WindowTop
    {
        get => _windowTop;
        set 
        {
            var maxTop = Math.Max(0, _screenHeight - _windowHeight);
            var newValue = Math.Clamp(value, 0, maxTop);
            
            this.RaiseAndSetIfChanged(ref _windowTop, newValue);
            this.RaisePropertyChanged(nameof(Position));
        }
    }

    public int MaxLeft => Math.Max(0, _screenWidth - _windowWidth);
    public int MaxTop => Math.Max(0, _screenHeight - _windowHeight);

    public PixelPoint Position => new(WindowLeft, WindowTop);

    public int WindowWidth
    {
        get => _windowWidth;
        set
        {
            this.RaiseAndSetIfChanged(ref _windowWidth, value);
            var maxLeft = Math.Max(0, _screenWidth - _windowWidth);
            if (_windowLeft > maxLeft)
            {
                WindowLeft = maxLeft;
            }
            this.RaisePropertyChanged(nameof(MaxLeft));
        }
    }

    public int WindowHeight
    {
        get => _windowHeight;
        set
        {
            this.RaiseAndSetIfChanged(ref _windowHeight, value);
            var maxTop = Math.Max(0, _screenHeight - _windowHeight);
            if (_windowTop > maxTop)
            {
                WindowTop = maxTop;
            }
            this.RaisePropertyChanged(nameof(MaxTop));
        }
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