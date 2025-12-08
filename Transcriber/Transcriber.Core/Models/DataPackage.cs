namespace Transcriber.Core.Models;

public class DataPackage(int size)
{
    public int BytesWritten { get; private set; }
    public byte[] Bytes { get; } = new byte[size];
    public bool IsFilled => BytesWritten == Bytes.Length;
    public bool IsEmpty => BytesWritten == 0;

    public int Write(byte[] bytes, int startIndex)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        
        var freeBytesCount = Bytes.Length - BytesWritten;
        var availableBytesCount = bytes.Length - startIndex;
        var bytesToTake = Math.Min(freeBytesCount, availableBytesCount);
        
        Array.Copy(
            sourceArray: bytes, 
            sourceIndex: startIndex,
            destinationArray: Bytes,
            destinationIndex: BytesWritten,
            length: bytesToTake);
        
        BytesWritten += bytesToTake;
        
        return bytesToTake;
    }
}