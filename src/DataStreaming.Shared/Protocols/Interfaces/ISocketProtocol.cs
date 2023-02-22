using System.Net.Sockets;

namespace DataStreaming.Protocols.Interfaces;

public interface ISocketProtocol
{
    Task DoCommunication(Socket party, CancellationToken token);
}