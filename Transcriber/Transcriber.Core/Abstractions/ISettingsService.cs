namespace Transcriber.Core.Abstractions;

public interface ISettingsService
{
    void RegisterSettings<T>(string name) where T : class;
    T GetSettings<T>() where T : class, new();
    void SaveSettings<T>(T settings) where T : class?;
}