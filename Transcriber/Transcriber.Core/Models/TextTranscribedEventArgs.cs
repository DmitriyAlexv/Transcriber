namespace Transcriber.Core.Models;

public class TextTranscribedEventArgs(TranscribeResult transcribeResult)
{
    public TranscribeResult TranscribeResult { get; set; } = transcribeResult;
}