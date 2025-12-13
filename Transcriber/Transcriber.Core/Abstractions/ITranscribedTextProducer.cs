using Transcriber.Core.Models;

namespace Transcriber.Core.Abstractions;

public interface ITranscribedTextProducer
{
    event EventHandler<TextTranscribedEventArgs>? OnTextTranscribed;
}