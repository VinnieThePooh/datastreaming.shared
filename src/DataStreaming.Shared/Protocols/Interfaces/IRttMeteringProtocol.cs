using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.RTT.Handlers;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Interfaces.RTT;

public interface IRttMeteringProtocol : IClientSocketProtocol
{
    RttMeteringType MeteringType { get; }

    IRttMeteringHandler MeteringHandler { get; }

    RttMeteringSettings MeteringSettings { get; }
}