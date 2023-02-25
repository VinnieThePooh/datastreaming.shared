using DataStreaming.Events;

namespace DataStreaming.Services.Interfaces;

public interface INotifyListeningStarted
{
    event AsyncEventHandler<ListeningEventArgs> ListeningStarted;
}