namespace DataStreaming.Events.Rtt;

public interface INotifyRttStatistics
{
    event AsyncEventHandler<RttStatisticsEventArgs> RttReceived;
}