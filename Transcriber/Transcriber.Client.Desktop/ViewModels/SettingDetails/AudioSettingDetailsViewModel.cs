using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.Services;
using Transcriber.Client.Desktop.ViewModels.Abstractions;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;
using Transcriber.Client.Desktop.ViewModels.Controls.Shared;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.ViewModels.SettingDetails;

public class AudioSettingDetailsViewModel : ViewModelBase, IActivatableViewModel, IHaveTitle
{
    public string Title => "Аудио";
    private AudioCaptureSettings _audioCaptureSettings;

    public AudioCaptureSettings AudioCaptureSettings
    {
        get => _audioCaptureSettings;
        set => this.RaiseAndSetIfChanged(ref _audioCaptureSettings, value);
    }
    
    public ViewModelActivator Activator { get; } = new();
    public ListEditorViewModel WhiteListEditor { get; }
    public ListEditorViewModel BlackListEditor { get; }

    public ObservableCollection<CaptureMode> CaptureModes { get; }
    public bool ShowWhiteList => AudioCaptureSettings.Mode is CaptureMode.WhiteList;
    public bool ShowBlackList => AudioCaptureSettings.Mode is CaptureMode.BlackList;
    public int WhiteListCount => AudioCaptureSettings.WhiteList.Count;
    public int BlackListCount => AudioCaptureSettings.BlackList.Count;

    public ObservableCollection<KeyValuePair<int, string>> AvailableDevices { get; }
    public bool ShowAudioDeviceSelectable => Singleton.AudioCaptureService is IDeviceSelectable;
    
    public string ModeDescription
    {
        get
        {
            return AudioCaptureSettings.Mode switch
            {
                CaptureMode.All => "Захватывать аудио со всех устройств",
                CaptureMode.WhiteList => "Захватывать аудио только из белого списка",
                CaptureMode.BlackList => "Захватывать аудио со всех устройств, кроме чёрного списка",
                _ => ""
            };
        }
    }

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<int, Unit> SetSampleRateCommand { get; }
    public ReactiveCommand<int, Unit> SetChannelsCommand { get; }

    public AudioSettingDetailsViewModel(SettingsNavigationViewModel settingsNavigationViewModel)
    {
        _audioCaptureSettings = AudioCaptureSettings = Copy(Singleton.AppSettingsManager.AudioCaptureSettings);

        var whiteList = new ObservableCollection<string>(_audioCaptureSettings.WhiteList);
        var blackList = new ObservableCollection<string>(_audioCaptureSettings.BlackList);

        WhiteListEditor = new ListEditorViewModel(whiteList);
        BlackListEditor = new ListEditorViewModel(blackList);

        WhiteListEditor.SourceCollection.CollectionChanged += (_, _) =>
        {
            _audioCaptureSettings.WhiteList = WhiteListEditor.SourceCollection.ToList();
            this.RaisePropertyChanged(nameof(WhiteListCount));
        };

        BlackListEditor.SourceCollection.CollectionChanged += (_, _) =>
        {
            _audioCaptureSettings.BlackList = BlackListEditor.SourceCollection.ToList();
            this.RaisePropertyChanged(nameof(BlackListCount));
        };

        CaptureModes =
        [
            CaptureMode.All,
            CaptureMode.WhiteList,
            CaptureMode.BlackList
        ];
        
        AvailableDevices = 
        [
            ..Singleton.AudioCaptureService is IDeviceSelectable deviceSelectable ? 
                deviceSelectable.GetAvailableDevices() : []
        ];

        NavigateBackCommand = ReactiveCommand.Create(() => 
        {
            Singleton.AppSettingsManager.AudioCaptureSettings = Copy(AudioCaptureSettings);
            settingsNavigationViewModel.NavigateBack();
        });

        SetSampleRateCommand = ReactiveCommand.Create<int>(rate => { AudioCaptureSettings.SampleRate = rate; });

        SetChannelsCommand = ReactiveCommand.Create<int>(channels => { AudioCaptureSettings.Channels = channels; });

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.AudioCaptureSettings.Mode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(ShowWhiteList));
                    this.RaisePropertyChanged(nameof(ShowBlackList));
                    this.RaisePropertyChanged(nameof(ModeDescription));
                })
                .DisposeWith(disposables);
        });
    }
    
    private AudioCaptureSettings Copy(AudioCaptureSettings audioCaptureSettings)
    {
        return new AudioCaptureSettings
        {
            Mode = audioCaptureSettings.Mode,
            WhiteList = audioCaptureSettings.WhiteList,
            BlackList = audioCaptureSettings.BlackList,
            Channels = audioCaptureSettings.Channels,
            SampleRate = audioCaptureSettings.SampleRate,
            CaptureDevice =  audioCaptureSettings.CaptureDevice,
            AudioCaptureOptions = audioCaptureSettings.AudioCaptureOptions
        };
    }
}