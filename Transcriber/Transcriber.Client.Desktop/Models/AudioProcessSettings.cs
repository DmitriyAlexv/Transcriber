using ReactiveUI;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Models;

public class AudioProcessSettings: ReactiveObject
{
    private DataProcessOptions _dataProcessOptions = new();
    private int _packageSize = 4800;
    private int _dueTime = 1000;
    private int _minPeriod = 100;
    private int _maxPeriod = 10000;

    public int PackageSize
    {
        get => _packageSize;
        set
        {
            this.RaiseAndSetIfChanged(ref _packageSize, value);
            DataProcessOptions.PackageSize = value;
        }
    }

    public int DueTime
    {
        get => _dueTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _dueTime, value);
            DataProcessOptions.DueTime = value;
        }
    }

    public int MinPeriod
    {
        get => _minPeriod;
        set
        {
            this.RaiseAndSetIfChanged(ref _minPeriod, value);
            DataProcessOptions.MinPeriod = value;
        }
    }

    public int MaxPeriod
    {
        get => _maxPeriod;
        set
        {
            this.RaiseAndSetIfChanged(ref _maxPeriod, value);
            DataProcessOptions.MaxPeriod = value;
        }
    }
    
    public DataProcessOptions DataProcessOptions
    {
        get => _dataProcessOptions;
        set
        {
            this.RaiseAndSetIfChanged(ref _dataProcessOptions, value);
            DataProcessOptions.PackageSize = value.PackageSize;
            DataProcessOptions.DueTime = value.DueTime;
            DataProcessOptions.MinPeriod = value.MinPeriod;
            DataProcessOptions.MaxPeriod = value.MaxPeriod;
        }
    }
}