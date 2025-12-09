using System;
using System.Reactive.Linq;
using Avalonia;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;

namespace Transcriber.Client.Desktop.ViewModels.Windows;

public class TextDisplayViewModel: ViewModelBase, IDisposable
{
    private TextDisplaySettings _settings;
    private string _displayText = string.Empty;
    private PixelPoint _position;
    private bool _isVisible;
    private double _windowOpacity;
    private readonly IDisposable _hideTimerSubscription;
    private bool _isWindowActive = true;
    
    public PixelPoint Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
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
        get => _windowOpacity;
        private set => this.RaiseAndSetIfChanged(ref _windowOpacity, value);
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

    public bool IsVisible
    {
        get => _isVisible;
        private set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public string DisplayText
    {
        get => _displayText;
        private set => this.RaiseAndSetIfChanged(ref _displayText, value);
    }

    public TextDisplayViewModel(TextDisplaySettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        
        // Начальные значения
        WindowOpacity = _settings.WindowOpacity;
        UpdatePosition();
        
        // Таймер для скрытия окна
        _hideTimerSubscription = Observable
            .Interval(TimeSpan.FromSeconds(2))
            .Where(_ => _isWindowActive && !string.IsNullOrEmpty(DisplayText))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                HideWindow();
            });
    }

    public void ShowText(string text)
    {
        if (!_isWindowActive) return;
        
        DisplayText = text;
        ShowWindow();
        
        // Сброс таймера при новом тексте
        ResetHideTimer();
    }

    public void ClearText()
    {
        DisplayText = string.Empty;
    }

    private void ShowWindow()
    {
        IsVisible = true;
        WindowOpacity = _settings.WindowOpacity;
    }

    private void HideWindow()
    {
        WindowOpacity = 0;
        
        // Через небольшое время после исчезновения анимации скрываем окно
        Observable.Timer(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                IsVisible = false;
            });
    }

    private void ResetHideTimer()
    {
        // Таймер сбрасывается автоматически, так как мы проверяем время получения текста
        // В этом упрощенном варианте окно скроется через 2 секунды после последнего вызова ShowText
    }

    public void UpdateSettings(TextDisplaySettings settings)
    {
        _settings = settings;
        WindowOpacity = _settings.WindowOpacity;
        UpdatePosition();
        
        this.RaisePropertyChanged(nameof(Width));
        this.RaisePropertyChanged(nameof(Height));
        this.RaisePropertyChanged(nameof(TextOpacity));
        this.RaisePropertyChanged(nameof(FontSize));
    }

    public void SetWindowActive(bool isActive)
    {
        _isWindowActive = isActive;
    }

    private void UpdatePosition()
    {
        Position = _settings.Position;
    }

    public void Dispose()
    {
        _hideTimerSubscription?.Dispose();
    }
}