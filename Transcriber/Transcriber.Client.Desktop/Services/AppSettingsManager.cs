using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Services;

public class AppSettingsManager: ReactiveObject
{
    private readonly ISettingsService _settingsService;
    private AudioCaptureSettings _audioCaptureSettings = null!;
    private AudioProcessSettings _audioProcessSettings = null!;
    private TextDisplaySettings _textDisplaySettings = null!;

    public AudioCaptureSettings AudioCaptureSettings
    {
        get => _audioCaptureSettings;
        set => this.RaiseAndSetIfChanged(ref _audioCaptureSettings, value);
    }

    public AudioProcessSettings AudioProcessSettings
    {
        get => _audioProcessSettings;
        set => this.RaiseAndSetIfChanged(ref _audioProcessSettings, value);
    }
    
    public TextDisplaySettings TextDisplaySettings
    {
        get => _textDisplaySettings;
        set => this.RaiseAndSetIfChanged(ref _textDisplaySettings, value);
    }

    public AppSettingsManager(ISettingsService settingsService)
    {
        settingsService.RegisterSettings<AudioCaptureOptions>("audio_settings.json");
        settingsService.RegisterSettings<DataProcessOptions>("audio_process_settings.json");
        settingsService.RegisterSettings<TextDisplaySettings>("text_display_settings.json");
        _settingsService = settingsService;
        AudioCaptureSettings = new AudioCaptureSettings();
        AudioProcessSettings = new AudioProcessSettings();
        AudioCaptureSettings.AudioCaptureOptions = _settingsService.GetSettings<AudioCaptureOptions>();
        AudioProcessSettings.DataProcessOptions = _settingsService.GetSettings<DataProcessOptions>();
        TextDisplaySettings = _settingsService.GetSettings<TextDisplaySettings>();
    }
    
    public void SaveAll()
    {
        _settingsService.SaveSettings(AudioCaptureSettings.AudioCaptureOptions);
        _settingsService.SaveSettings(AudioProcessSettings.DataProcessOptions);
        _settingsService.SaveSettings(TextDisplaySettings);
    }
}