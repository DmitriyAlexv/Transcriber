namespace Transcriber.Core.Abstractions;

public interface IDeviceSelectable
{
    Dictionary<int, string> GetAvailableDevices();
}