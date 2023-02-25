using System.Net;
using System.Net.Sockets;
using DataStreaming.Models.RTT;
using DataStreaming.Protocols.Factories;
using DataStreaming.Services.Interfaces;
using DataStreaming.Settings;

namespace DataStreaming.Services.RTT;

//todo: logger?
public class RttMeteringServer : INetworkService<HostSettings>, IHasClientProxies<RttClientProxy>, IAsyncDisposable
{
    private readonly ISocketProtocolFactory protocolFactory;
    private CancellationTokenSource? cts;
    private Socket? serverSocket;

    public RttMeteringServer(HostSettings settings, ISocketProtocolFactory protocolFactory)
    {
        this.protocolFactory = protocolFactory ?? throw new ArgumentNullException(nameof(protocolFactory));
        HostSettings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<bool> Start()
    {
        if (cts is not null)
            return false;

        cts = new CancellationTokenSource();

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(HostSettings.Host), HostSettings.Port));
        serverSocket.Listen();

        while (!cts.IsCancellationRequested)
        {
            var client = await serverSocket.AcceptAsync(cts.Token);
            var proxy = CreateProxy(client, cts.Token);
            ClientProxies.Add(proxy.EndPoint, proxy);
            _ = Task.Run(() => proxy.DoCommunication(proxy.TokenSource!.Token));
        }
        
        return true;
    }

    public async Task<bool> Stop()
    {
        if (cts is null)
            return false;
        
        cts.Cancel();
        cts = null;

        await DisposeAsync();
        return true;
    }

    public HostSettings HostSettings { get; }

    public Dictionary<IPEndPoint, RttClientProxy> ClientProxies { get; } = new();
    
    public RttClientProxy CreateProxy(Socket party, CancellationToken token)
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        return new RttClientProxy(party, tokenSource, protocolFactory.CreateServerProtocol());
    }

    public ValueTask DisposeAsync()
    {
        serverSocket?.Dispose();
        cts?.Dispose();
        
        foreach (var clientProxy in ClientProxies.Values)
        {
            if (!clientProxy.TokenSource.IsCancellationRequested)
                clientProxy.TokenSource.Cancel();
            clientProxy.Dispose();
        }
        ClientProxies.Clear();
        return ValueTask.CompletedTask;
    }
}