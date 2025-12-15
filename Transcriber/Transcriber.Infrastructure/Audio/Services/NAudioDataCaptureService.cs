using NAudio.CoreAudioApi;
using NAudio.Wave;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Infrastructure.Audio.Services;

public class NAudioDataCaptureService : IDataCaptureService
{
    private readonly object _captureLock = new();
    private readonly AudioCaptureOptions _audioCaptureOptions;
    private readonly WasapiLoopbackCapture _capture;

    private DataCaptureState _state = DataCaptureState.Stopped;
    public DataCaptureState State
    {
        get => _state;
        private set
        {
            _state = value;
            OnStatusChanged?.Invoke(this, new CaptureStatusChangedEventArgs());
        }
        
    }
    
    public event EventHandler<DataCapturedEventArgs>? OnDataCaptured;
    public event EventHandler<CaptureStatusChangedEventArgs>? OnStatusChanged;

    public NAudioDataCaptureService(AudioCaptureOptions audioCaptureOptions)
    {
        _audioCaptureOptions = audioCaptureOptions;
        _capture = new WasapiLoopbackCapture()
        {
            WaveFormat = new WaveFormat(audioCaptureOptions.SampleRate, audioCaptureOptions.Channels)
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