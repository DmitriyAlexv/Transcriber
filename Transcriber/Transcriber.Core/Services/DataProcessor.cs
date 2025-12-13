using System.Collections.Concurrent;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class DataProcessor: IDataProcessor, IDisposable
{
    private readonly object _currentDataPackageLock = new();
    private readonly DataProcessSettings _dataProcessSettings;
    private readonly ConcurrentQueue<DataPackage> _dataPackagesQueue = new();
    private readonly List<IDataPackageProcessor> _dataPackageProcessors;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private DateTime _lastDataPackageProcessTime = DateTime.UtcNow;
    private DataPackage _currentDataPackage;

    public DataProcessor(DataProcessSettings dataProcessSettings, List<IDataPackageProcessor> dataPackageProcessors)
    {
        _dataProcessSettings = dataProcessSettings;
        _dataPackageProcessors = dataPackageProcessors;
        _currentDataPackage = new DataPackage(dataProcessSettings.PackageSize);
        _dataPackagesQueue.Enqueue(_currentDataPackage);
        
        _ = Task.Run(async () =>
            await ProcessDataPackageQueueLoopAsync(_cancellationTokenSource.Token));
    }
    
    public void ReceiveData(object? sender, DataCapturedEventArgs data)
    {
        var dataBytes = data.Bytes; 
        for (var i = 0; i < dataBytes.Length;)
        {
            lock (_currentDataPackageLock)
            {
                i += _currentDataPackage.Write(dataBytes, i);

                if (!_currentDataPackage.IsFilled)
                    continue;
                
                PrepareNewPackage();
            }
        }
    }
    
    private async Task ProcessDataPackageQueueLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_dataProcessSettings.MinPeriod));

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessDataPackageQueueAsync();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
    
    private async Task ProcessDataPackageQueueAsync()
    {
        if(_dataPackagesQueue.IsEmpty)
            return;
        
        var maxPeriodExceeded = MaxPeriodExceeded();
        
        if (_dataPackagesQueue.TryPeek(out var dataPackagePeek) && 
            (dataPackagePeek.IsFilled || maxPeriodExceeded) &&
            _dataPackagesQueue.TryDequeue(out var dataPackage))
        {
            if (maxPeriodExceeded)
            {
                lock (_currentDataPackageLock)
                {
                    if (_dataPackagesQueue.IsEmpty)
                    {
                        PrepareNewPackage();
                    }
                }
            }
            
            await ProcessDataPackage(dataPackage);
        }
    }

    private void PrepareNewPackage()
    {
        _currentDataPackage = new DataPackage(_dataProcessSettings.PackageSize);
        _dataPackagesQueue.Enqueue(_currentDataPackage);
    }
    
    private bool MaxPeriodExceeded()
        => (DateTime.UtcNow - _lastDataPackageProcessTime).TotalMilliseconds >= _dataProcessSettings.MaxPeriod;

    private async Task ProcessDataPackage(DataPackage dataPackage)
    {
        _lastDataPackageProcessTime = DateTime.UtcNow;
        if(dataPackage.IsEmpty)
            return;

        var tasks = _dataPackageProcessors
            .Select(processor => processor.ProcessDataPackageAsync(dataPackage))
            .ToList();
    
        await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}