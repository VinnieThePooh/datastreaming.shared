using DataStreaming.Constants.RTT;

namespace DataStreaming.Settings;

public class RttMeteringSettings : HostSettings
{
    public const string SectionName = "RTTMetering";

    public RttMeteringType MeteringType { get; set; }

    public HandlerMeteringSettings SinglePacket { get; set; }

    public HandlerMeteringSettings AggregationInterval { get; set; }
}