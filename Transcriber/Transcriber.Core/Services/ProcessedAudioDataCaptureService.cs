using System.Net.Sockets;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class ProcessedAudioDataCaptureService: IDataCaptureService
{
    private const string Host = "localhost";
    private const int Port = 9998;
    
    private TcpClient? _client;
    private NetworkStream? _stream;
    
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
    
    
    public void StartCapture()
    {
        if(State is not DataCaptureState.Stopped) 
            return;
        
        State = DataCaptureState.Starting;
        _ = ConnectAndStartReceivingAsync();
    }

    public void StopCapture()
    {
        if(State is not DataCaptureState.Started)
            return;
        
        DisposeConnection();
        State = DataCaptureState.Stopping;
    }
    
    private async Task ConnectAndStartReceivingAsync()
    {
        if (_stream == null ||  _client is not { Connected: true })
            await ReconnectAsync();
        
        _ = Task.Run(ReceiveLoopAsync);
    }

    private async Task ReconnectAsync()
    {
        DisposeConnection();
        await ConnectAsync();
    }
    
    private async Task ConnectAsync()
    {
        _client = new TcpClient();
        await _client.ConnectAsync(Host, Port);
        _stream = _client.GetStream();
    }
    
    private async Task ReceiveLoopAsync()
    {
        try
        {
            var buffer = new byte[4096];
            State = DataCaptureState.Started;
            
            while (State is DataCaptureState.Started && _stream != null && _client is { Connected: true })
            {
                var bytesRead = await _stream.ReadAsync(buffer);

                if (bytesRead <= 0)
                    continue;
                
                var processedAudioData = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, processedAudioData, 0, bytesRead);
                
                OnDataCaptured?.Invoke(this, new DataCapturedEventArgs(processedAudioData));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка приема данных: {ex.Message}");
        }
        finally
        {
            State = DataCaptureState.Stopped;
        }
    }
        
    private void DisposeConnection()
    {
        _stream?.Dispose();
        _client?.Dispose();
        _stream = null;
        _client = null;
    }
    
    public void Dispose()
    {
        DisposeConnection();
    }
}