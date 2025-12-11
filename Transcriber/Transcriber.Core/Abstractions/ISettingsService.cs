namespace Transcriber.Core.Abstractions;

public interface ISettingsService
{
    void RegisterSettings<T>(string name) where T : class;
    Task<T> GetSettingsAsync<T>() where T : class, new();
    Task SaveSettingsAsync<T>(T settings) where T : class?;
}