using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Interfaces.RTT;

public interface IRttMeteringProtocol
{
    RttMeteringType MeteringType { get; }

    IRttMeteringHandler MeteringHandler { get; }

    RttMeteringSettings Settings { get; }
}