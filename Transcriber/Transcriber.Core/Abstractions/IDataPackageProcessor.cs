using Transcriber.Core.Models;

namespace Transcriber.Core.Abstractions;

public interface IDataPackageProcessor
{
    Task ProcessDataPackageAsync(DataPackage dataPackage);
}