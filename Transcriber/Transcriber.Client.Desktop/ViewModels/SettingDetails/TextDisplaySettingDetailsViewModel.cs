using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.Services;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels.SettingDetails;

public class TextDisplaySettingDetailsViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly TextDisplayPreviewViewModel _textDisplayPreviewViewModel;
    
    private TextDisplaySettings _settings;
    private TextDisplayPreviewWindow? _textDisplayPreviewWindow;
    private string _previewText = "Пример текста для preview. Этот текст будет меняться в реальном времени.";
    
    public ViewModelActivator Activator { get; } = new();

    public TextDisplaySettings Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    public string PreviewText
    {
        get => _previewText;
        set => this.RaiseAndSetIfChanged(ref _previewText, value);
    }

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> GenerateRandomPreviewTextCommand { get; }

    public TextDisplaySettingDetailsViewModel(SettingsNavigationViewModel settingsNavigationViewModel)
    {
        Settings = Copy(Singleton.AppSettingsManager.TextDisplaySettings);

        _textDisplayPreviewViewModel = new TextDisplayPreviewViewModel(Settings);

        NavigateBackCommand = ReactiveCommand.Create(() => 
        {
            Singleton.AppSettingsManager.TextDisplaySettings = Copy(Settings);
            settingsNavigationViewModel.NavigateBack();
        });

        GenerateRandomPreviewTextCommand = ReactiveCommand.Create(GenerateRandomPreviewText);

        this.WhenActivated(disposables =>
        {
            ShowPreviewWindow();

            var settingsSubscription = this.WhenAnyValue(
                x => x.Settings.WindowLeft,
                x => x.Settings.WindowTop,
                x => x.Settings.WindowWidth,
                x => x.Settings.WindowHeight,
                x => x.Settings.WindowOpacity,
                x => x.Settings.TextOpacity,
                x => x.Settings.FontSize)
                .Throttle(TimeSpan.FromMilliseconds(10))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdatePreviewSettings());

            var textSubscription = this.WhenAnyValue(x => x.PreviewText)
                .Subscribe(UpdatePreviewText);

            Disposable.Create(ClosePreviewWindow)
                .DisposeWith(disposables);

            settingsSubscription.DisposeWith(disposables);
            textSubscription.DisposeWith(disposables);
        });
    }

    private void GenerateRandomPreviewText()
    {
        var texts = new[]
        {
            "Пример текста для отображения",
            "Это preview окно показывает настройки в реальном времени",
            "Изменение размера шрифта, прозрачности и положения окна",
            "Текст для тестирования отображения настроек",
            "Динамическое обновление preview при изменении параметров"
        };

        var random = new Random();
        PreviewText = texts[random.Next(texts.Length)];
    }

    private void ShowPreviewWindow()
    {
        if (_textDisplayPreviewWindow is { IsVisible: true }) 
            return;
        
        _textDisplayPreviewWindow = new TextDisplayPreviewWindow { DataContext = _textDisplayPreviewViewModel };

        _textDisplayPreviewWindow.Closed += (_, _) => _textDisplayPreviewWindow = null;
        _textDisplayPreviewWindow.Show();
    }

    private void UpdatePreviewSettings()
    {
        if (_textDisplayPreviewViewModel != null)
        {
            _textDisplayPreviewViewModel.Settings = Settings;
        }
    }

    private void UpdatePreviewText(string text)
    {
        if (_textDisplayPreviewViewModel != null)
        {
            _textDisplayPreviewViewModel.DisplayText = text;
        }
    }

    private void ClosePreviewWindow()
    {
        if (_textDisplayPreviewWindow == null) 
            return;
        
        Singleton.AppSettingsManager.TextDisplaySettings = Copy(Settings);
        _textDisplayPreviewWindow.Close();
        _textDisplayPreviewWindow = null;
    }

    private TextDisplaySettings Copy(TextDisplaySettings textDisplaySettings)
    {
        return new TextDisplaySettings
        {
            WindowLeft = textDisplaySettings.WindowLeft,
            WindowTop = textDisplaySettings.WindowTop,
            WindowWidth = textDisplaySettings.WindowWidth,
            WindowHeight = textDisplaySettings.WindowHeight,
            WindowOpacity = textDisplaySettings.WindowOpacity,
            TextOpacity = textDisplaySettings.TextOpacity,
            FontSize = textDisplaySettings.FontSize
        };
    }
}