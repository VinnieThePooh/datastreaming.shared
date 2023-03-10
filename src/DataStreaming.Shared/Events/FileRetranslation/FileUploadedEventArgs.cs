using System.Net;

namespace DataStreaming.Events;

public class FileUploadedEventArgs
{
    public IPEndPoint Uploader { get; init; }

    public byte[] FileData { get; init; }

    public byte[] FileNameData { get; init; }

    //number of messages within the batch
    public int BatchSize { get; init; }

    //within the batch of messages
    public int MessageOrderNumber { get; init; }
}