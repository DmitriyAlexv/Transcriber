using System.Runtime.InteropServices;
using PortAudioSharp;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;
using Stream = PortAudioSharp.Stream;

namespace Transcriber.Infrastructure.Audio.Services;

public class PortAudioCaptureService : IDataCaptureService
{
    private readonly object _captureLock = new();
    private readonly AudioCaptureOptions _audioCaptureOptions;
    private readonly Stream _stream;

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

    private static readonly object StaticLock = new();
    private static int _initCount;

    public PortAudioCaptureService(AudioCaptureOptions audioCaptureOptions)
    {
        _audioCaptureOptions = audioCaptureOptions;
        EnsureInitialized();

        var device = audioCaptureOptions.DeviceId;
        
        var outParams = new StreamParameters
        {
            device = device,
            channelCount = audioCaptureOptions.Channels,
            sampleFormat = SampleFormat.Int16,
            suggestedLatency = PortAudio.GetDeviceInfo(device).defaultLowInputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        _stream = new Stream(
            inParams: outParams,
            outParams: null,
            sampleRate: audioCaptureOptions.SampleRate,
            framesPerBuffer: 0,
            streamFlags: StreamFlags.ClipOff,
            callback: AudioCallback,
            userData: IntPtr.Zero
        );
    }

    public void StartCapture()
    {
        lock (_captureLock)
        {
            if (_state != DataCaptureState.Stopped)
                return;

            State = DataCaptureState.Starting;
            _stream.Start();
        }
    }

    public void StopCapture()
    {
        lock (_captureLock)
        {
            if (_state != DataCaptureState.Started)
                return;

            State = DataCaptureState.Stopping;
            _stream.Stop();
            State = DataCaptureState.Stopped;
        }
    }

    public void Dispose()
    {
        lock (_captureLock)
        {
            if (_stream is not null)
            {
                if (_stream.IsActive)
                    _stream.Stop();
                _stream.Close();
            }

            EnsureTerminated();
        }
    }

    private StreamCallbackResult AudioCallback(
        IntPtr input,
        IntPtr output,
        uint frameCount,
        ref StreamCallbackTimeInfo timeInfo,
        StreamCallbackFlags statusFlags,
        IntPtr userData)
    {
        HandleAudioData(input, frameCount);
        
        return StreamCallbackResult.Continue;
    }

    private void HandleAudioData(IntPtr input, uint frameCount)
    {
        if (_state == DataCaptureState.Starting)
        {
            lock (_captureLock)
            {
                if (_state == DataCaptureState.Starting)
                    State = DataCaptureState.Started;
            }
        }

        if (OnDataCaptured == null || frameCount == 0)
            return;

        var bytesPerFrame = _audioCaptureOptions.Channels * sizeof(short);
        var totalBytes = (int)(frameCount * bytesPerFrame);
        var audioData = new byte[totalBytes];

        Marshal.Copy(input, audioData, 0, totalBytes);
        OnDataCaptured?.Invoke(this, new DataCapturedEventArgs(audioData));
    }

    private static void EnsureInitialized()
    {
        lock (StaticLock)
        {
            if (_initCount == 0)
                PortAudio.Initialize();
            _initCount++;
        }
    }

    private static void EnsureTerminated()
    {
        lock (StaticLock)
        {
            _initCount--;
            if (_initCount == 0)
                PortAudio.Terminate();
        }
    }
}