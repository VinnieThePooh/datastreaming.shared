using System.Net;

namespace DataStreaming.Events;

public class ListeningEventArgs : EventArgs
{
    public ListeningEventArgs(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    public IPEndPoint EndPoint { get; }
}