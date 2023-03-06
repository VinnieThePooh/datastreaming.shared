using System.Net;
using System.Net.Sockets;
using DataStreaming.Exceptions;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringServerProto : IServerSocketProtocol
{
    public async Task DoCommunication(Socket party, CancellationToken token)
    {
        var memory = new Memory<byte>(new byte[64 * 1024]);
        while (!token.IsCancellationRequested)
        {
            var read = await party.ReceiveAsync(memory, token);
            if (read is 0)
            {
                var ep = (IPEndPoint)party.RemoteEndPoint!;
                throw new DisconnectedPrematurelyException($"Client {ep.Address}:{ep.Port} disconnected");
            }

            await party.SendAsync(memory[..read], token);
        }

        token.ThrowIfCancellationRequested();
    }
}