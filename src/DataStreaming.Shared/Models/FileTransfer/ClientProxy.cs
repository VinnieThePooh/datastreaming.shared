using System.Net;
using System.Net.Sockets;
using DataStreaming.Constants;
using DataStreaming.Events;
using DataStreaming.Protocols;
using DataStreaming.Services.FileTransfer;

namespace DataStreaming.Models.FileTransfer;

public class ClientProxy
{
    public ClientProxy(IPEndPoint endPoint, RetranslationServerProto protocol)
    {
        EndPoint = endPoint;
        Protocol = protocol;
    }

    public TcpClient Client { get; private set; }

    public IPEndPoint EndPoint { get; }

    public ClientType ClientType { get; private set; }

    public RetranslationServerProto Protocol { get; }

    internal void SetClient(TcpClient client)
    {
        Client = client;
    }

    public Task DoCommunication(CancellationToken token)
    {
        if (Client is null)
            throw new InvalidOperationException("Client was not initialized");

        Protocol.ClientTypeDetected += OnClientTypeDetected;
        return Task.Run(() => Protocol.DoCommunication(Client, token), token);
    }

    private void OnClientTypeDetected(object? sender, ClientTypeDetectedEventArgs e)
    {
        Console.WriteLine($"[{nameof(RetranslationServer)}]: New client type detected: {e.Type} ({EndPoint})");
        ClientType = e.Type;
        Protocol.ClientTypeDetected -= OnClientTypeDetected;
    }
}