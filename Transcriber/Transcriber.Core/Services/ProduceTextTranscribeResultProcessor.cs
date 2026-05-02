using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class ProduceTextTranscribeResultProcessor: IDataObjectProcessor<TranscribeResult>, ITranscribedTextProducer
{
    public event EventHandler<TextTranscribedEventArgs>? OnTextTranscribed;
    
    public Task ProcessDataObjectAsync(TranscribeResult dataObject)
    {
        OnTextTranscribed?.Invoke(this, new TextTranscribedEventArgs(dataObject));
        return Task.CompletedTask;
    }
}