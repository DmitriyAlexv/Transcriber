namespace Transcriber.Core.Models;

public class DataCapturedEventArgs
{
    public byte[] Bytes { get; set; }

    public DataCapturedEventArgs(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        
        Bytes = bytes;
    }
}