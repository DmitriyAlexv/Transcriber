using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class HandledDataProcessor<TData> : IDataProcessor, IDisposable
{
    private readonly List<IDataObjectProcessor<TData>> _dataObjectProcessors;
    private readonly ConcurrentQueue<byte[]> _dataBytesQueue = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    public HandledDataProcessor(List<IDataObjectProcessor<TData>> dataObjectProcessors)
    {
        _dataObjectProcessors = dataObjectProcessors;
        
        _ = Task.Run(async () =>
            await ProcessDataBytesQueueLoopAsync(_cancellationTokenSource.Token));
    }
    
    public void ReceiveData(object? sender, DataCapturedEventArgs data)
    {
        _dataBytesQueue.Enqueue(data.Bytes);
    }
    
    private async Task ProcessDataBytesQueueLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));

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
        if(_dataBytesQueue.IsEmpty)
            return;
        
        if (_dataBytesQueue.TryDequeue(out var dataBytes))
        {
            await ProcessDataBytes(dataBytes);
        }
    }
    
    private async Task ProcessDataBytes(byte[] dataBytes)
    {
        if(dataBytes.Length == 0)
            return;

        try
        {
            var jsonString = Encoding.UTF8.GetString(dataBytes);
            var lines = jsonString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                try
                {
                    var dataObject = JsonSerializer.Deserialize<TData>(line);
                    
                    if (dataObject != null)
                    {
                        var tasks = _dataObjectProcessors
                            .Select(processor => processor.ProcessDataObjectAsync(dataObject))
                            .ToList();
                        
                        await Task.WhenAll(tasks);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка десериализации {line}; {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки данных: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}