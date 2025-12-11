using System.Threading.Tasks;
using Transcriber.Client.Desktop.Models;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Services;

public class AppSettingsManager
{
    private readonly ISettingsService _settingsService;
    public AudioSettings? AudioSettings { get; set; }
    public DataProcessSettings? AudioProcessSettings { get; set; }
    public TextDisplaySettings? TextDisplaySettings { get; set; }

    public AppSettingsManager(ISettingsService settingsService)
    {
        settingsService.RegisterSettings<AudioSettings>("audio_settings.json");
        settingsService.RegisterSettings<AudioSettings>("audio_process_settings.json");
        settingsService.RegisterSettings<AudioSettings>("text_display_settings.json");
        _settingsService = settingsService;
    }

    public async Task LoadAllAsync()
    {
        AudioSettings = await _settingsService.GetSettingsAsync<AudioSettings>();
        AudioProcessSettings = await _settingsService.GetSettingsAsync<DataProcessSettings>();
        TextDisplaySettings = await _settingsService.GetSettingsAsync<TextDisplaySettings>();
    }
    
    public async Task SaveAllAsync()
    {
        await _settingsService.SaveSettingsAsync(AudioSettings);
        await _settingsService.SaveSettingsAsync(AudioProcessSettings);
        await _settingsService.SaveSettingsAsync(TextDisplaySettings);
    }
}