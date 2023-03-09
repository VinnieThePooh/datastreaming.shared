namespace DataStreaming.Models.RTT;

public struct AggregatedRttStats
{
    /// <summary>
    /// Min number of arrived packet in a sequence
    /// </summary>
    public ulong SequenceNumber { get; set; }
    
    /// <summary>
    /// Barrier phase number
    /// </summary>
    public long PhaseNumber { get; set; }

    /// <summary>
    /// Average RTT in the interval
    /// </summary>
    public TimeSpan AvgRtt { get; set; }

    public TimeSpan MaxRtt { get; set; }

    public TimeSpan MinRtt { get; set; }

    //in ms
    public int AggregationInterval { get; set; }

    public int PacketsCount { get; set; }
}