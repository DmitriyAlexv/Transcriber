using System.Collections.Generic;
using ReactiveUI;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Models;

public class AudioCaptureSettings: ReactiveObject
{
    private AudioCaptureOptions _audioCaptureOptions = new();
    private CaptureMode _mode = CaptureMode.All;
    private List<string> _whiteList = [];
    private List<string> _blackList = [];
    private int _sampleRate = 16000;
    private int _channels = 1;
    private KeyValuePair<int, string> _captureDevice = new(0, "Audio Capture Device");

    public CaptureMode Mode
    {
        get => _mode;
        set
        {
            this.RaiseAndSetIfChanged(ref _mode, value);
            AudioCaptureOptions.Mode = value;
        }
    }

    public List<string> WhiteList
    {
        get => _whiteList;
        set
        {
            this.RaiseAndSetIfChanged(ref _whiteList, value);
            AudioCaptureOptions.WhiteList = value;
        }
    }

    public List<string> BlackList
    {
        get => _blackList;
        set
        {
            this.RaiseAndSetIfChanged(ref _blackList, value);
            AudioCaptureOptions.BlackList = value;
        }
    }

    public int SampleRate
    {
        get => _sampleRate;
        set
        {
            this.RaiseAndSetIfChanged(ref _sampleRate, value);
            AudioCaptureOptions.SampleRate = value;
        }
    }

    public int Channels
    {
        get => _channels;
        set
        {
            this.RaiseAndSetIfChanged(ref _channels, value);
            AudioCaptureOptions.Channels = value;
        }
    }
    
    public KeyValuePair<int, string> CaptureDevice
    {
        get => _captureDevice;
        set
        {
            this.RaiseAndSetIfChanged(ref _captureDevice, value);
            AudioCaptureOptions.CaptureDevice = value;
        }
    }
    
    public AudioCaptureOptions AudioCaptureOptions
    {
        get => _audioCaptureOptions;
        set
        {
            this.RaiseAndSetIfChanged(ref _audioCaptureOptions, value);
            Mode = value.Mode;
            WhiteList = value.WhiteList;
            BlackList = value.BlackList;
            SampleRate = value.SampleRate;
            Channels = value.Channels;
            CaptureDevice = value.CaptureDevice;
        }
    }
}