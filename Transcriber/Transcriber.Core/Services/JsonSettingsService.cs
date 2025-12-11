using System.Text.Json;
using Transcriber.Core.Abstractions;

namespace Transcriber.Core.Services;

public class JsonSettingsService : ISettingsService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
    {
        WriteIndented = true
    };
    private readonly Dictionary<Type, string> _fileMappings = new();
    
    public void RegisterSettings<T>(string name) where T : class
    {
        _fileMappings.Add(typeof(T), name);
    }

    public async Task<T> GetSettingsAsync<T>() where T : class, new()
    {
        var filePath = GetFilePath<T>();
        return File.Exists(filePath) 
            ? JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(filePath)) ?? new T()
            : new T();
    }

    public async Task SaveSettingsAsync<T>(T settings) where T : class?
    {
        if(settings == null)
            return;
        
        var filePath = GetFilePath<T>();
        var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
    
    private string GetFilePath<T>()
    {
        var type = typeof(T);
        if (!_fileMappings.TryGetValue(type, out var filename))
            throw new ArgumentException($"No mapping for type {type.Name}");
        
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YourApp",
            "Settings",
            filename
        );
    }
}