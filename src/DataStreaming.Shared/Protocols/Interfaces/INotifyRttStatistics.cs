using DataStreaming.Events;
using DataStreaming.Events.Rtt;

namespace DataStreaming.Protocols.Interfaces;

public interface INotifyRttStatistics
{
    event AsyncEventHandler<RttStatisticsEventArgs> RttReceived;
}