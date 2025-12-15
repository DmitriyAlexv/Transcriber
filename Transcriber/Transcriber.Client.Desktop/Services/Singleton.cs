using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;
using Transcriber.Core.Services;
using Transcriber.Infrastructure.Audio.Services;

namespace Transcriber.Client.Desktop.Services;

public static class Singleton
{
    public static readonly ISettingsService SettingsService = new JsonSettingsService();
    public static readonly AppSettingsManager AppSettingsManager = new (SettingsService);
    public static readonly IDataCaptureService AudioCaptureService;
    public static readonly IDataCaptureService TranscribedTextCaptureService;
    public static readonly ITranscribedTextProducer TranscribedTextProducer;

    static Singleton()
    {
        var transcribedTextDataPackageProcessor = new ProduceTextDataObjectProcessor();
        var randomTextDataObjectProcessor = new RandomTextDataPackageProcessor();
        TranscribedTextProducer = transcribedTextDataPackageProcessor;
        
        var dataProcessor = new DataProcessor(
            AppSettingsManager.AudioProcessSettings.DataProcessOptions,
            [
                //new NAudioSaveDataPackageProcessor(AppSettingsManager.AudioSettings),
                //randomTextDataObjectProcessor,
                new AudioSendDataPackageProcessor()
            ]);
        
        var transcribedDataProcessor = new HandledDataProcessor<TranscribeResult>(
            [
                transcribedTextDataPackageProcessor
            ]);
        
        AudioCaptureService = new NAudioDataCaptureService(AppSettingsManager.AudioCaptureSettings.AudioCaptureOptions);
        AudioCaptureService.OnDataCaptured += dataProcessor.ReceiveData;

        TranscribedTextCaptureService = new ProcessedAudioDataCaptureService();
        TranscribedTextCaptureService.OnDataCaptured += transcribedDataProcessor.ReceiveData;
    }
}