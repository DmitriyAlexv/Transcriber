namespace Transcriber.Core.Abstractions;

public interface IDataObjectProcessor<in TData>
{
    Task ProcessDataObjectAsync(TData dataObject);
}