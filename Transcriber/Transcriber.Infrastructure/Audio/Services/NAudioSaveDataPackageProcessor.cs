using NAudio.Wave;
using Transcriber.Core.Abstractions;
using Transcriber.Core.Models;

namespace Transcriber.Infrastructure.Audio.Services;

public class NAudioSaveDataPackageProcessor(AudioSettings audioSettings) : IDataPackageProcessor
{
    private const string OutputDirectory = @"C:\Dima\ADO\Transcriber\Transcriber\TestAudio";
    private readonly List<DataPackage> _dataPackages = [];
    private readonly WaveFormat _waveFormat = new(audioSettings.SampleRate, audioSettings.Channels);

    public async Task ProcessDataPackageAsync(DataPackage dataPackage)
    {
        if (_dataPackages.Count < 20)
        {
            _dataPackages.Add(dataPackage);
            return;
        }
        
        var bytes = _dataPackages.SelectMany(d => d.IsFilled ? d.Bytes : d.Bytes.Take(d.BytesWritten))
            .ToArray();
        _dataPackages.Clear();
        
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = Path.Combine(OutputDirectory, $"audio_{timestamp}.wav");
            
        await using var writer = new WaveFileWriter(filename, _waveFormat);
        await writer.WriteAsync(bytes);
    }
}