using System.Net.Sockets;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringServerProto : IServerProtocol
{
    public Task DoCommunication(TcpClient party, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}