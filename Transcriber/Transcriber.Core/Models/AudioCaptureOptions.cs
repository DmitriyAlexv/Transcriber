namespace Transcriber.Core.Models;

public class AudioCaptureOptions
{
    public CaptureMode Mode { get; set; } = CaptureMode.All;
    public List<string> WhiteList { get; set; } = [];
    public List<string> BlackList { get; set; } = [];
    public int SampleRate { get; set; } = 44100;
    public int Channels { get; set; } = 2;
    public int DeviceId { get; set; } = 37;
}