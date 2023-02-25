using System.Diagnostics;

namespace DataStreaming.Models.RTT;

public struct RttStats
{
    public ulong SequenceNumber { get; set; }

    public TimeSpan Rtt { get; set; }

    public long SendTimeTrace { get; set; }

    public static RttStats WithCurrentTimetrace(ulong counter) => new() { SequenceNumber = counter, SendTimeTrace = Stopwatch.GetTimestamp() };
}