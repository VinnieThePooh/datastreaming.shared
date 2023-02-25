using System.Net;
using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Events;
using DataStreaming.Events.Rtt;
using DataStreaming.Protocols.Factories;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Protocols.Interfaces.RTT;
using DataStreaming.Services.Interfaces;
using DataStreaming.Settings;

namespace DataStreaming.Services.RTT;

public class RttMeteringService : IRttMeteringService
{
    private bool disposed;
    private CancellationTokenSource? cts;
    private Socket? clientSocket;
    private readonly IRttMeteringProtocol clientMeteringProtocol;
    
    RttMeteringSettings INetworkService<RttMeteringSettings>.Settings => MeteringSettings;

    public RttMeteringService(RttMeteringSettings settings, IRttMeteringProtocolFactory factory)
    {
        _ = settings ?? throw new ArgumentNullException(nameof(settings));
        clientMeteringProtocol = factory.CreateClientProtocol(settings);
    }

    public RttMeteringType MeteringType => clientMeteringProtocol.MeteringType;
    
    public IRttMeteringHandler MeteringHandler => clientMeteringProtocol.MeteringHandler;
    public RttMeteringSettings MeteringSettings => clientMeteringProtocol.MeteringSettings;
    
    public event AsyncEventHandler<RttStatisticsEventArgs>? RttReceived;

    public async Task<bool> Start()
    {
        if (cts is not null)
            return false;
        
        cts = new();
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientMeteringProtocol.MeteringHandler.RttReceived += OnRttReceived;

        var settings = clientMeteringProtocol.MeteringSettings;
        await clientSocket.ConnectAsync(IPAddress.Parse(settings.Host), settings.Port, cts.Token);
        await clientMeteringProtocol.DoCommunication(clientSocket, cts.Token);
        return true;
    }

    public async Task<bool> Stop()
    {
        if (cts is null)
            return false;
        
        cts?.Cancel();
        cts = null;
        await DisposeAsync();
        return true;
    }

    public RttMeteringSettings HostSettings { get; }

    public ValueTask DisposeAsync()
    {
        if (disposed)
            return ValueTask.CompletedTask;

        disposed = true;

        MeteringHandler.RttReceived -= OnRttReceived;
        cts?.Cancel();
        cts = null;
        clientSocket.Dispose();
        clientSocket = null;
        return ValueTask.CompletedTask;
    }

    private Task OnRttReceived(object sender, RttStatisticsEventArgs eventArgs) => RttReceived?.Invoke(this, eventArgs);
}