using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Handlers.RTT;

public interface IRttMeteringHandler : ISocketProtocol
{
    RttMeteringType MeteringType { get; }
    
    Task ReceivingTask { get; }
}