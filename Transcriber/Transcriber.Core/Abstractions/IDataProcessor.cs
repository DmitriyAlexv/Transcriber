using Transcriber.Core.Models;

namespace Transcriber.Core.Abstractions;

public interface IDataProcessor
{
    void ReceiveData(object? sender, DataCapturedEventArgs data);
}