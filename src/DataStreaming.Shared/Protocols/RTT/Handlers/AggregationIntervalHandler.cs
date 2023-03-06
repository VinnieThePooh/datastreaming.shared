using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Events;
using DataStreaming.Events.RTT;

namespace DataStreaming.Protocols.Handlers.RTT;

public class AggregationIntervalHandler : IRttMeteringHandler
{
    public Task DoCommunication(Socket party, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public event AsyncEventHandler<RttStatisticsEventArgs>? RttReceived;

    public RttMeteringType MeteringType => RttMeteringType.AggregationInterval;

    public Task ReceivingTask { get; }
}