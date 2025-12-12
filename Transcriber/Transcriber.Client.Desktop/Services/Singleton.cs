using Transcriber.Core.Abstractions;
using Transcriber.Core.Services;
using Transcriber.Infrastructure.Audio.Services;

namespace Transcriber.Client.Desktop.Services;

public static class Singleton
{
    public static readonly ISettingsService SettingsService = new JsonSettingsService();
    public static readonly AppSettingsManager AppSettingsManager = new (SettingsService);
    public static readonly IDataCaptureService AudioCaptureService;
    public static readonly ITextProducer TextProducer;

    static Singleton()
    {
        var textDataPackageProcessor = new TextDataPackageProcessor();
        TextProducer = textDataPackageProcessor;
        var dataProcessor = new DataProcessService(
            AppSettingsManager.AudioProcessSettings,
            [
                new NAudioSaveDataPackageProcessor(AppSettingsManager.AudioSettings),
                textDataPackageProcessor
            ]);
        AudioCaptureService = new NAudioDataCaptureService(AppSettingsManager.AudioSettings);
        AudioCaptureService.OnDataCaptured += dataProcessor.ReceiveData;
    }
}