using Transcriber.Core.Abstractions;
using Transcriber.Core.Services;

namespace Transcriber.Client.Desktop.Services;

public static class Singleton
{
    public static readonly ISettingsService SettingsService = new JsonSettingsService();
    public static readonly AppSettingsManager AppSettingsManager = new (SettingsService);
}