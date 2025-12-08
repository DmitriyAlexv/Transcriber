using Transcriber.Core.Models;

namespace Transcriber.Core.Abstractions;

public interface IDataCaptureService: IDisposable
{
    DataCaptureState State { get; }
    
    event EventHandler<DataCapturedEventArgs>? OnDataCaptured;
    
    void StartCapture();
    void StopCapture();
}