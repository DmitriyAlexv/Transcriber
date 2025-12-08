namespace Transcriber.Core.Models;

public class DataProcessSettings
{
    public int PackageSize { get; set; } = 4800;
    public int DueTime { get; set; } = 1000;
    public int MinPeriod { get; set; } = 100;
    public int MaxPeriod { get; set; } = 10000;
}