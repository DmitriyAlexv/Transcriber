using Transcriber.Core.Models;

namespace Transcriber.Core.Abstractions;

public interface IDataCaptureService: IDisposable
{
    DataCaptureState State { get; }
    
    event EventHandler<DataCapturedEventArgs>? OnDataCaptured;
    event EventHandler<CaptureStatusChangedEventArgs>? OnStatusChanged;
    
    void StartCapture();
    void StopCapture();
}