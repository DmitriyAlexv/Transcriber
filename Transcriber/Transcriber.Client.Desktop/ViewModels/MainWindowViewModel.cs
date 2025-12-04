using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;
using Transcriber.Core.Services;
using Transcriber.Infrastructure.Audio.Services;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IDataCaptureService _audioDataCaptureService;
    private IDataProcessor _audioDataProcessor;
    private List<IDataPackageProcessor> _audioDataPackageProcessors;
    private bool _isCapturing;
    private string _captureButtonText = "Начать захват";
    private string _selectedTab = "main";

    public MainWindowViewModel()
    {
        ToggleCaptureCommand = ReactiveCommand.Create(ToggleCapture);
        ShowSettingsCommand = ReactiveCommand.Create(() => SelectedTab = "settings");
        ShowTextSettingsCommand = ReactiveCommand.Create(() => SelectedTab = "textSettings");
        ShowMainCommand = ReactiveCommand.Create(() => SelectedTab = "main");
        MinimizeCommand = ReactiveCommand.Create(MinimizeApp);
        ExitCommand = ReactiveCommand.Create(ExitApp);

        AudioSettings = new AudioSettings();
        TextSettings = new TextDisplaySettings();
        
        _audioDataCaptureService = new NAudioDataCaptureService(AudioSettings);
        var textDataPackageProcessor = new TextDataPackageProcessor();
        _audioDataPackageProcessors =
        [
            new NAudioSaveDataPackageProcessor(AudioSettings),
            textDataPackageProcessor
        ];
        _audioDataProcessor = new DataProcessService(new DataProcessSettings(), _audioDataPackageProcessors);
        _audioDataCaptureService.OnDataCaptured += _audioDataProcessor.ReceiveData;
        textDataPackageProcessor.TextConverted += OnTextConverted;
    }

    public bool IsCapturing
    {
        get => _isCapturing;
        set => this.RaiseAndSetIfChanged(ref _isCapturing, value);
    }

    public string CaptureButtonText
    {
        get => _captureButtonText;
        set => this.RaiseAndSetIfChanged(ref _captureButtonText, value);
    }

    public string SelectedTab
    {
        get => _selectedTab;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTab, value);
            this.RaisePropertyChanged(nameof(IsMainTabVisible));
            this.RaisePropertyChanged(nameof(IsSettingsTabVisible));
            this.RaisePropertyChanged(nameof(IsTextSettingsTabVisible));
        }
    }
    
    public bool IsMainTabVisible => SelectedTab == "main";
    public bool IsSettingsTabVisible => SelectedTab == "settings";
    public bool IsTextSettingsTabVisible => SelectedTab == "textSettings";

    public AudioSettings AudioSettings { get; }
    public TextDisplaySettings TextSettings { get; }

    public ObservableCollection<string> TextLines { get; } = [];
    private const int MaxTextLines = 10;

    public ICommand ToggleCaptureCommand { get; }
    public ICommand ShowSettingsCommand { get; }
    public ICommand ShowTextSettingsCommand { get; }
    public ICommand ShowMainCommand { get; }
    public ICommand MinimizeCommand { get; }
    public ICommand ExitCommand { get; }

    private void ToggleCapture()
    {
        if (IsCapturing)
        {
            _audioDataCaptureService.StartCapture();
            CaptureButtonText = "Остановить захват";
        }
        else
        {
            _audioDataCaptureService.StopCapture();
            CaptureButtonText = "Начать захват";
        }
    }

    private void OnTextConverted(object? sender, string text)
    {
        TextLines.Insert(0, $"{DateTime.Now:HH:mm:ss}: {text}");

        if (TextLines.Count > MaxTextLines)
        {
            TextLines.RemoveAt(TextLines.Count - 1);
        }
    }

    private void MinimizeApp()
    {
        // Логика минимизации будет в MainWindow
    }

    private void ExitApp()
    {
        _audioDataCaptureService.StopCapture();
        Environment.Exit(0);
    }
}