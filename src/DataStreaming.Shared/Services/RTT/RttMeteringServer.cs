using System.Net;
using System.Net.Sockets;
using DataStreaming.Events;
using DataStreaming.Models.RTT;
using DataStreaming.Protocols.Factories;
using DataStreaming.Services.Interfaces;
using DataStreaming.Settings;

namespace DataStreaming.Services.RTT;

//todo: logger?
public class RttMeteringServer : INetworkService<HostSettings>, IHasClientProxies<RttClientProxy>,
    INotifyListeningStarted, IAsyncDisposable
{
    private readonly ISocketProtocolFactory protocolFactory;
    private CancellationTokenSource? cts;
    private Socket? serverSocket;

    public RttMeteringServer(HostSettings settings, ISocketProtocolFactory protocolFactory)
    {
        this.protocolFactory = protocolFactory ?? throw new ArgumentNullException(nameof(protocolFactory));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public event AsyncEventHandler<ListeningEventArgs>? ListeningStarted;

    public async Task<bool> Start()
    {
        if (cts is not null)
            return false;

        cts = new CancellationTokenSource();

        var endPoint = new IPEndPoint(IPAddress.Parse(Settings.Host), Settings.Port);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(endPoint);
        serverSocket.Listen();
        ListeningStarted?.Invoke(this, new ListeningEventArgs(endPoint));

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

    public HostSettings Settings { get; }

    public Dictionary<IPEndPoint, RttClientProxy> ClientProxies { get; } = new();

    public RttClientProxy CreateProxy(Socket party, CancellationToken token)
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        return new RttClientProxy(party, tokenSource, protocolFactory.CreateServerProtocol());
    }

    public ValueTask DisposeAsync()
    {
        serverSocket.Shutdown(SocketShutdown.Both);
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