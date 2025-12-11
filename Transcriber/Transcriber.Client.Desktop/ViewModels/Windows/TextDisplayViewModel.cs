using System;
using System.Reactive.Linq;
using Avalonia;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;

namespace Transcriber.Client.Desktop.ViewModels.Windows;

public class TextDisplayViewModel: ViewModelBase, IDisposable
{
    private IDisposable _currentHideTimer;
    private TextDisplaySettings _settings;
    private string _displayText = string.Empty;
    private PixelPoint _position;
    private bool _isVisible;
    
    public PixelPoint Position
    {
        get => _position;
        set
        {
            this.RaiseAndSetIfChanged(ref _position, value);
        }
    }


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

    public bool IsVisible
    {
        get => _isVisible;
        private set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }


    public TextDisplayViewModel(TextDisplaySettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        UpdatePosition();
        
        // Таймер для скрытия окна
        _currentHideTimer = Observable
            .Timer(TimeSpan.FromSeconds(2))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (IsVisible && !string.IsNullOrEmpty(DisplayText))
                {
                    HideWindow();
                }
            });
    }

    public void ShowText(string text)
    {
        Console.WriteLine($"showText: {text}");
        
        // Отменяем предыдущий таймер
        _currentHideTimer?.Dispose();
        
        DisplayText = text;
        ShowWindow();
        
        // Запускаем новый таймер скрытия
        _currentHideTimer = Observable
            .Timer(TimeSpan.FromSeconds(2))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (IsVisible && !string.IsNullOrEmpty(DisplayText))
                {
                    HideWindow();
                }
            });

    }

    public void ClearText()
    {
        DisplayText = string.Empty;
    }

    private void ShowWindow()
    {
        IsVisible = true;
    }

    private void HideWindow()
    {
        IsVisible = false;
    }

    private void ResetHideTimer()
    {
        // Таймер сбрасывается автоматически, так как мы проверяем время получения текста
        // В этом упрощенном варианте окно скроется через 2 секунды после последнего вызова ShowText
    }

    public void UpdateSettings(TextDisplaySettings settings)
    {
        _settings = settings;
        UpdatePosition();
        
        this.RaisePropertyChanged(nameof(Width));
        this.RaisePropertyChanged(nameof(Height));
        this.RaisePropertyChanged(nameof(TextOpacity));
        this.RaisePropertyChanged(nameof(FontSize));
    }

    private void UpdatePosition()
    {
        Position = _settings?.Position ?? new PixelPoint(100, 100);
    }

    public void Dispose()
    {
        _currentHideTimer?.Dispose();
    }
}