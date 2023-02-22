using System.Net.Sockets;
using DataStreaming.Constants.RTT;

namespace DataStreaming.Settings;

public class RttMeteringSettings
{
    public const string SectionName = "RTTMetering";

    public string Host { get; set; }

    public int Port { get; set; }

    public RttMeteringType MeteringType { get; set; }

    public HandlerMeteringSettings SinglePacket { get; set; }

    public HandlerMeteringSettings AggregationInterval { get; set; }
}

public struct HandlerMeteringSettings
{
    public HandlerMeteringSettings()
    {        
    }
    
    public int PacketSize { get; set; }

    public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;

    //in ms
    public int Interval { get; set; }
}