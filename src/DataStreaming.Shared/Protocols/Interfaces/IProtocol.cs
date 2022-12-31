using System.Net.Sockets;

namespace DataStreaming.Protocols.Interfaces;

public interface IProtocol
{
    Task DoCommunication(TcpClient party, CancellationToken token);
}