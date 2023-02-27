using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.RTT.Handlers;

public interface IRttMeteringHandler : ISocketProtocol, INotifyRttStatistics
{
    RttMeteringType MeteringType { get; }
    
    Task ReceivingTask { get; }
}