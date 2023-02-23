using System.Net.Sockets;

namespace DataStreaming.Settings;

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