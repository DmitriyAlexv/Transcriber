using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Services;

public class AppSettingsManager: ReactiveObject
{
    private readonly ISettingsService _settingsService;
    public AudioSettings AudioSettings { get; set; }
    public DataProcessSettings AudioProcessSettings { get; set; }

    private TextDisplaySettings _textDisplaySettings;
    public TextDisplaySettings TextDisplaySettings
    {
        get => _textDisplaySettings;
        set => this.RaiseAndSetIfChanged(ref _textDisplaySettings, value);
    }

    public AppSettingsManager(ISettingsService settingsService)
    {
        settingsService.RegisterSettings<AudioSettings>("audio_settings.json");
        settingsService.RegisterSettings<DataProcessSettings>("audio_process_settings.json");
        settingsService.RegisterSettings<TextDisplaySettings>("text_display_settings.json");
        _settingsService = settingsService;
        AudioSettings = _settingsService.GetSettings<AudioSettings>();
        AudioProcessSettings = _settingsService.GetSettings<DataProcessSettings>();
        TextDisplaySettings = _settingsService.GetSettings<TextDisplaySettings>();
    }
    
    public void SaveAll()
    {
        _settingsService.SaveSettings(AudioSettings);
        _settingsService.SaveSettings(AudioProcessSettings);
        _settingsService.SaveSettings(TextDisplaySettings);
    }
}