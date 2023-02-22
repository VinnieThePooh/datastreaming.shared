using DataStreaming.Constants;

namespace DataStreaming.Events;

public class ClientTypeDetectedEventArgs
{
    public ClientTypeDetectedEventArgs(ClientType type)
    {
        Type = type;
    }

    public ClientType Type { get; }
}