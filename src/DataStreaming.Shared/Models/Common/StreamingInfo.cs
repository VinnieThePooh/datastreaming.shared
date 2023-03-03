namespace DataStreaming.Models.Common;

public interface StreamingInfo<out T>
{
    Memory<byte> MessageBuffer { get; set; }

    Memory<byte> LeftData { get; set; }

    int? PacketSize { get; set; }
    
    bool IsDisconnectedPrematurely { get; }

    T Message { get; }

    void ConstructMessage(MemoryStream? memory = null);
}