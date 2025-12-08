using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transcriber.Core.Services;

public static class SettingsSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task SaveSettingsAsync<T>(string filePath, T settings)
    {
        var json = JsonSerializer.Serialize(settings, Options);
        await File.WriteAllTextAsync(filePath, json);
    }

    public static async Task<T?> LoadSettingsAsync<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default;

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}