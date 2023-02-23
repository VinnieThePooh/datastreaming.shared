namespace DataStreaming.Models.RTT;

public struct IntervalRttStats
{
    public ulong SequenceNumber { get; set; }

    public TimeSpan RttValue { get; set; }

    //in ms
    public int AggregationInterval { get; set; }

    public int PacketsCount { get; set; }
}