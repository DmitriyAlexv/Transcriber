using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels.SettingDetails;

public class TextDisplaySettingDetailsViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly SettingsNavigationViewModel _settingsNavigationViewModel;
    private TextDisplaySettings _settings;
    private string _previewText = "Пример текста для preview. Этот текст будет меняться в реальном времени.";
    private TextDisplayPreviewWindow _previewWindow;
    private TextDisplayPreviewViewModel _viewModel;

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
        _settingsNavigationViewModel = settingsNavigationViewModel;

        // Загружаем настройки
        Settings = LoadSettings();

        // Создаем ViewModel для preview окна
        _viewModel = new TextDisplayPreviewViewModel(Settings);

        NavigateBackCommand = ReactiveCommand.Create(() => 
        {
            SaveSettings();
            _settingsNavigationViewModel.NavigateBack();
        });

        GenerateRandomPreviewTextCommand = ReactiveCommand.Create(GenerateRandomPreviewText);

        // Управляем preview окном через активацию/деактивацию ViewModel
        this.WhenActivated(disposables =>
        {
            // Открываем preview окно при активации
            ShowPreviewWindow();

            // Обновляем preview при изменении настроек
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

            // Обновляем текст в preview
            var textSubscription = this.WhenAnyValue(x => x.PreviewText)
                .Subscribe(text => UpdatePreviewText(text));

            // Закрываем preview при деактивации
            Disposable.Create(() => ClosePreviewWindow())
                .DisposeWith(disposables);

            settingsSubscription.DisposeWith(disposables);
            textSubscription.DisposeWith(disposables);
        });
    }

    private TextDisplaySettings LoadSettings()
    {
        // В реальном приложении загружаем из настроек
        return new TextDisplaySettings();
    }

    private void SaveSettings()
    {
        // В реальном приложении сохраняем настройки
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
        if (_previewWindow == null || !_previewWindow.IsVisible)
        {
            _previewWindow = new TextDisplayPreviewWindow
            {
                DataContext = _viewModel,
                CanResize = false,
                ShowInTaskbar = false,
                Topmost = true,
                ExtendClientAreaToDecorationsHint = true,
                TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur]
            };

            _previewWindow.Closed += (s, e) => _previewWindow = null;
            _previewWindow.Show();
        }
    }

    private void UpdatePreviewSettings()
    {
        if (_viewModel != null)
        {
            _viewModel.UpdateSettings(Settings);
        }
    }

    private void UpdatePreviewText(string text)
    {
        if (_viewModel != null)
        {
            _viewModel.DisplayText = text;
        }
    }

    private void ClosePreviewWindow()
    {
        if (_previewWindow != null)
        {
            _previewWindow.Close();
            _previewWindow = null;
        }
    }
}