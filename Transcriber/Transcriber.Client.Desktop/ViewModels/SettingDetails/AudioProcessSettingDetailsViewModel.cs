using System.Reactive;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.Services;
using Transcriber.Client.Desktop.ViewModels.Abstractions;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

namespace Transcriber.Client.Desktop.ViewModels.SettingDetails;

public class AudioProcessSettingDetailsViewModel: ViewModelBase, IHaveTitle
{
    private AudioProcessSettings _audioProcessSettings;
    
    public string Title => "Обработка";
    
    public AudioProcessSettings Settings
    {
        get => _audioProcessSettings;
        set => this.RaiseAndSetIfChanged(ref _audioProcessSettings, value);
    }
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    
    public AudioProcessSettingDetailsViewModel(SettingsNavigationViewModel settingsNavigationViewModel)
    {
        Settings = Copy(Singleton.AppSettingsManager.AudioProcessSettings);
        
        NavigateBackCommand = ReactiveCommand.Create(() => 
        {
            Singleton.AppSettingsManager.AudioProcessSettings = Copy(Settings);
            settingsNavigationViewModel.NavigateBack();
        });
    }

    private AudioProcessSettings Copy(AudioProcessSettings audioProcessSettings)
    {
        return new AudioProcessSettings
        {
            PackageSize = audioProcessSettings.PackageSize,
            DueTime = audioProcessSettings.DueTime,
            MaxPeriod = audioProcessSettings.MaxPeriod,
            MinPeriod = audioProcessSettings.MinPeriod,
            DataProcessOptions = audioProcessSettings.DataProcessOptions,
        };
    }
}