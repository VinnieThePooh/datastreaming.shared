using DataStreaming.Constants.RTT;
using DataStreaming.Events.Rtt;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Handlers.RTT;

public interface IRttMeteringHandler : ISocketProtocol, INotifyRttStatistics
{
    RttMeteringType MeteringType { get; }
    
    Task ReceivingTask { get; }
}