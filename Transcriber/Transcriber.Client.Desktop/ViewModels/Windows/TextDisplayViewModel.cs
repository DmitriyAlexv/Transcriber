using System;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.Services;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.ViewModels.Windows;

public class TextDisplayViewModel: ViewModelBase, IDisposable
{
    private IDisposable _currentHideTimer;
    private string _displayText = string.Empty;
    private bool _isVisible;
    
    public TextDisplaySettings Settings => Singleton.AppSettingsManager.TextDisplaySettings;

    public string DisplayText
    {
        get => _displayText;
        private set => this.RaiseAndSetIfChanged(ref _displayText, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        private set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }


    public TextDisplayViewModel()
    {
        Observable.FromEventPattern<EventHandler<TextTranscribedEventArgs>, TextTranscribedEventArgs>(
                h => Singleton.TranscribedTextProducer.OnTextTranscribed += h,
                h => Singleton.TranscribedTextProducer.OnTextTranscribed -= h)
            .Select(e => e.EventArgs.TranscribeResult)
            .ObserveOn(RxApp.MainThreadScheduler) 
            .Subscribe(ShowTextFromTranscribeResult);
        
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
        _currentHideTimer.Dispose();
        
        DisplayText = text;
        ShowWindow();

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

    private void ShowTextFromTranscribeResult(TranscribeResult result)
    {
        ShowText(result.Text!);
    }

    private void ShowWindow()
    {
        IsVisible = true;
    }

    private void HideWindow()
    {
        IsVisible = false;
    }

    public void Dispose()
    {
        _currentHideTimer.Dispose();
    }
}