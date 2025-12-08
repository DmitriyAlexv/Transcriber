using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class TextDataPackageProcessor: IDataPackageProcessor, ITextProducer
{
    private readonly Random _random = new();
    
    public event EventHandler<string>? TextConverted;

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
        TextConverted?.Invoke(this, randomText);
        return Task.CompletedTask;
    }
}