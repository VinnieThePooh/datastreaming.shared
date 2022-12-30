using System.Net.Sockets;

namespace ImageRetranslationShared.Protocols.Interfaces;

public interface IProtocol
{
    Task DoCommunication(TcpClient party, CancellationToken token);
}