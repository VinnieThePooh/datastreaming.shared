using DataStreaming.Constants.RTT;
using DataStreaming.Models.RTT;

namespace DataStreaming.Events.Rtt;

public class RttStatisticsEventArgs : EventArgs
{
    public RttStatisticsEventArgs(ulong counter, RttStats rttStats)
    {
        SequenceNumber = counter;
        MeteringType = RttMeteringType.SinglePacket;
        RttStats = rttStats;
    }

    public RttStatisticsEventArgs(ulong counter, AggregatedRttStats? aggregatedRttStats)
    {
        SequenceNumber = counter;
        MeteringType = RttMeteringType.AggregationInterval;
        AggregatedRttStats = aggregatedRttStats;
    }

    public ulong SequenceNumber { get; }

    public RttMeteringType MeteringType { get; }

    public RttStats? RttStats { get; }

    public AggregatedRttStats? AggregatedRttStats { get; }
}