namespace Transcriber.Core.Abstractions;

public interface ITextProducer
{
    event EventHandler<string>? TextConverted;
}