namespace DataStreaming.Models.Common;

public interface StreamingInfo<T>
{
    public Memory<byte> MessageBuffer { get; set; }

    public Memory<byte> LeftData { get; set; }

    public int? PacketSize { get; set; }

    public T Message { get; }

    void ConstructMessage();
}