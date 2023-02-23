namespace DataStreaming.Models.RTT;

public struct AggregatedRttStats
{
    /// <summary>
    /// Lowest number of arrived packet in a sequence
    /// </summary>
    public ulong SequenceNumber { get; set; }

    public TimeSpan RttValue { get; set; }

    //in ms
    public int AggregationInterval { get; set; }

    public int PacketsCount { get; set; }
}