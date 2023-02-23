namespace DataStreaming.Models.RTT;

public struct RttResponse
{
    public ulong SequenceNumber { get; set; }

    public long Timetrace { get; set; }

    public string Text { get; set; }
}