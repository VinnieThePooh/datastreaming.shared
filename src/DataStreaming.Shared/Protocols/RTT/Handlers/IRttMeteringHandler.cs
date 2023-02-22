using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Handlers.RTT;

public interface IRttMeteringHandler : IProtocol
{
    RttMeteringType MeteringType { get; }
}