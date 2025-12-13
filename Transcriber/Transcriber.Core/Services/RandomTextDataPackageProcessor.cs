using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

// For test only
public class RandomTextDataPackageProcessor: IDataPackageProcessor, ITranscribedTextProducer
{
    private readonly Random _random = new();

    public event EventHandler<TextTranscribedEventArgs>? OnTextTranscribed;

    public Task ProcessDataPackageAsync(DataPackage dataPackage)
    {
        var simulatedTexts = new[]
        {
            "Привет, как дела?",
            "Сегодня хорошая погода",
            "Тестируем захват аудио",
            "Это демонстрационный текст",
            "Аудио поток обработан успешно"
        };

        var randomText = simulatedTexts[_random.Next(simulatedTexts.Length)];
        OnTextTranscribed?.Invoke(this, new TextTranscribedEventArgs(new TranscribeResult(){Text = randomText, Type = "final"}));
        return Task.CompletedTask;
    }
}