using System.Text.Json.Serialization;

namespace Transcriber.Core.Models;

public class TranscribeResult
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}