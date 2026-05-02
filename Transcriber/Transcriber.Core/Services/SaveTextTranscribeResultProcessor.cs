using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class SaveTextTranscribeResultProcessor: IDataObjectProcessor<TranscribeResult>
{
    private string _fileName = GetNewFileName(); 
    
    public async Task ProcessDataObjectAsync(TranscribeResult dataObject)
    {
        if (dataObject.Type?.ToLower() != "final")
            return;
        
        CreateDirectory();
        
        var path = GetFilePath();
        var text = FormatText(dataObject.Text);
        
        await File.AppendAllTextAsync(path, text);
    }

    public void HandleNewTranscriptProcess(object? sender, CaptureStatusChangedEventArgs e)
    {
        _fileName = GetNewFileName();
    }

    private static string GetNewFileName() 
        => $"transcript_{DateTime.Now:HH-mm-ss}_{Guid.NewGuid()}.txt";

    private static string FormatText(string? text)
        => text != null ? $"\n[{DateTime.Now:HH:mm:ss}] - {text}" : string.Empty;

    private string GetFilePath() 
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Transcriber",
            "Transcripts",
            _fileName
        );
    
    private void CreateDirectory()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Transcriber",
            "Transcripts");
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}