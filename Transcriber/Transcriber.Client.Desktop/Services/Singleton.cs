using Transcriber.Core.Abstractions;
using Transcriber.Core.Services;
using Transcriber.Infrastructure.Audio.Services;

namespace Transcriber.Client.Desktop.Services;

public static class Singleton
{
    public static readonly ISettingsService SettingsService = new JsonSettingsService();
    public static readonly AppSettingsManager AppSettingsManager = new (SettingsService);
    public static readonly IDataCaptureService AudioCaptureService;
    public static readonly ITranscribedTextProducer TranscribedTextProducer;

    static Singleton()
    {
        var textDataPackageProcessor = new RandomTextDataObjectProcessor();
        TranscribedTextProducer = textDataPackageProcessor;
        var dataProcessor = new DataProcessor(
            AppSettingsManager.AudioProcessSettings,
            [
                new NAudioSaveDataPackageProcessor(AppSettingsManager.AudioSettings),
                textDataPackageProcessor
            ]);
        AudioCaptureService = new NAudioDataCaptureService(AppSettingsManager.AudioSettings);
        AudioCaptureService.OnDataCaptured += dataProcessor.ReceiveData;
    }
}