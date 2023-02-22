using System.Net.Sockets;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringServerProto : IServerSocketProtocol
{
    public Task DoCommunication(Socket party, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}