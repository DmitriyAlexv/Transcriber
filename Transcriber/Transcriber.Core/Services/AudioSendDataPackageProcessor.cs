using System.Net.Sockets;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Core.Services;

public class AudioSendDataPackageProcessor : IDataPackageProcessor, IDisposable
{
    private const string Host = "localhost";
    private const int Port = 9999;
    
    private TcpClient? _client;
    private NetworkStream? _stream;

    public async Task ProcessDataPackageAsync(DataPackage dataPackage)
    {
        if (dataPackage.IsEmpty)
            return;
        
        if (_stream == null || _client is not { Connected: true })
            await ReconnectAsync();

        try
        {
            await _stream!.WriteAsync(dataPackage.Bytes.AsMemory(0, dataPackage.BytesWritten));
        }
        catch (IOException e) when (e.InnerException is SocketException { SocketErrorCode: SocketError.ConnectionAborted })
        {
            await ReconnectAsync();
            await _stream!.WriteAsync(dataPackage.Bytes.AsMemory(0, dataPackage.BytesWritten));
        }
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