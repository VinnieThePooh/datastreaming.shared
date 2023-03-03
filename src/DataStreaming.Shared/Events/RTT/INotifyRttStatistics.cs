namespace DataStreaming.Events.RTT;

public interface INotifyRttStatistics
{
    event AsyncEventHandler<RttStatisticsEventArgs> RttReceived;
}