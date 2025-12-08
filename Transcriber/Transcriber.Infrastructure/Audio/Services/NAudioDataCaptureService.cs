using NAudio.CoreAudioApi;
using NAudio.Wave;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Infrastructure.Audio.Services;

public class NAudioDataCaptureService : IDataCaptureService
{
    private readonly object _captureLock = new();
    private readonly AudioSettings _audioSettings;
    private readonly WasapiLoopbackCapture _capture;

    public DataCaptureState State { get; private set; } = DataCaptureState.Stopped;
    
    public event EventHandler<DataCapturedEventArgs>? OnDataCaptured;

    public NAudioDataCaptureService(AudioSettings audioSettings)
    {
        _audioSettings = audioSettings;
        _capture = new WasapiLoopbackCapture()
        {
            WaveFormat = new WaveFormat(audioSettings.SampleRate, audioSettings.Channels)
        };
        _capture.DataAvailable += OnDataAvailable;
        _capture.RecordingStopped += OnStopped;
    }
    
    public void StartCapture()
    {
        lock (_captureLock)
        {
            if (_capture.CaptureState != CaptureState.Stopped)
                return;

            State = DataCaptureState.Starting;
            _capture.StartRecording();
        }
    }

    public void StopCapture()
    {
        lock (_captureLock)
        {
            if (_capture.CaptureState != CaptureState.Capturing)
                return;

            State = DataCaptureState.Stopping;
            _capture.StopRecording();
        }
    }
    
    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (State == DataCaptureState.Starting)
            State = DataCaptureState.Started;
        
        if (OnDataCaptured == null || e.BytesRecorded == 0)
            return;
        
        var audioData = new byte[e.BytesRecorded];
        Buffer.BlockCopy(e.Buffer, 0, audioData, 0, e.BytesRecorded);
            
        OnDataCaptured?.Invoke(this, new DataCapturedEventArgs(audioData));
    }

    private void OnStopped(object? sender, StoppedEventArgs e)
    {
        if(State == DataCaptureState.Stopping)
            State = DataCaptureState.Stopped;
    }
    
    public void Dispose()
    {
        lock (_captureLock)
        {
            _capture.Dispose();
        }
    }
}