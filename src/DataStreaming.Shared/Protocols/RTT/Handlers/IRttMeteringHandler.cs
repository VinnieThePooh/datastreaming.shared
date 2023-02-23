using DataStreaming.Constants.RTT;
using DataStreaming.Events;
using DataStreaming.Events.Rtt;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Handlers.RTT;

public interface IRttMeteringHandler : ISocketProtocol
{
    AsyncEventHandler<RttStatisticsEventArgs> RttReceived { get; }

    RttMeteringType MeteringType { get; }
    
    Task ReceivingTask { get; }
}