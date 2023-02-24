using System.Net;
using System.Net.Sockets;
using DataStreaming.Models.Common;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Models.RTT;

public class RttClientProxy : ISocketClientProxy
{
    private bool disposed;
    
    public RttClientProxy(Socket party, CancellationTokenSource tokenSource, IServerSocketProtocol serverProtocol)
    {
        Party = party;
        TokenSource = tokenSource;
        EndPoint = (IPEndPoint)party.RemoteEndPoint!;
        ServerProtocol = serverProtocol;
    }

    public CancellationTokenSource? TokenSource { get; private set; }
    public Socket? Party { get; private set; }
    public IPEndPoint EndPoint { get; }
    public IServerSocketProtocol ServerProtocol { get; }

    public Task DoCommunication(CancellationToken token) => ServerProtocol.DoCommunication(Party, token);

    public void Dispose()
    {
        if (disposed)
            return;
        
        disposed = true;

        Party.Dispose();
        TokenSource.Dispose();
        Party = null;
        TokenSource = null;
    }
}