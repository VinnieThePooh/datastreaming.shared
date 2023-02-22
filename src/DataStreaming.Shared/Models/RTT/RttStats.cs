namespace DataStreaming.Models.RTT;

public struct RttStats
{
    public ulong SequenceNumber { get; set; }

    public TimeSpan RttValue { get; set; }
}