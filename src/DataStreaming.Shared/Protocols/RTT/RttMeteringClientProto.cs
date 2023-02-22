using System.Net.Sockets;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringClientProto : IClientProtocol
{
    public Task DoCommunication(TcpClient party, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}