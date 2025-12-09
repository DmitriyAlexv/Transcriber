using Avalonia;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;

namespace Transcriber.Client.Desktop.ViewModels.Windows;

public class TextDisplayPreviewViewModel: ViewModelBase
{
    private TextDisplaySettings _settings;
    private string _displayText = "Пример текста для preview";
    private PixelPoint _position;

    public PixelPoint Position
    {
        get => _position;
        set
        {
            this.RaiseAndSetIfChanged(ref _position, value);
            this.RaisePropertyChanged(nameof(PositionString));
        }
    }

    public string PositionString => $"{_position.X} {_position.Y}";

    public double Width
    {
        get => _settings?.WindowWidth ?? 400;
        set
        {
            if (_settings != null)
            {
                _settings.WindowWidth = value;
                this.RaisePropertyChanged(nameof(Width));
            }
        }
    }

    public double Height
    {
        get => _settings?.WindowHeight ?? 200;
        set
        {
            if (_settings != null)
            {
                _settings.WindowHeight = value;
                this.RaisePropertyChanged(nameof(Height));
            }
        }
    }

    public double WindowOpacity
    {
        get => _settings?.WindowOpacity ?? 0.8;
        set
        {
            if (_settings != null)
            {
                _settings.WindowOpacity = value;
                this.RaisePropertyChanged(nameof(WindowOpacity));
            }
        }
    }

    public double TextOpacity
    {
        get => _settings?.TextOpacity ?? 1.0;
        set
        {
            if (_settings != null)
            {
                _settings.TextOpacity = value;
                this.RaisePropertyChanged(nameof(TextOpacity));
            }
        }
    }

    public int FontSize
    {
        get => _settings?.FontSize ?? 14;
        set
        {
            if (_settings != null)
            {
                _settings.FontSize = value;
                this.RaisePropertyChanged(nameof(FontSize));
            }
        }
    }

    public string DisplayText
    {
        get => _displayText;
        set => this.RaiseAndSetIfChanged(ref _displayText, value);
    }

    public TextDisplayPreviewViewModel(TextDisplaySettings settings)
    {
        _settings = settings;
        UpdatePosition();
    }

    public void UpdateSettings(TextDisplaySettings settings)
    {
        _settings = settings;
        UpdatePosition();
        
        this.RaisePropertyChanged(nameof(Width));
        this.RaisePropertyChanged(nameof(Height));
        this.RaisePropertyChanged(nameof(WindowOpacity));
        this.RaisePropertyChanged(nameof(TextOpacity));
        this.RaisePropertyChanged(nameof(FontSize));
    }

    private void UpdatePosition()
    {
        Position = _settings?.Position ?? new PixelPoint(100, 100);
    }
}