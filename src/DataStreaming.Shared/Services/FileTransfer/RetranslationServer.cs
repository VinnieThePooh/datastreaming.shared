using System.Net;
using System.Net.Sockets;
using System.Text;
using DataStreaming.Constants;
using DataStreaming.Events;
using DataStreaming.Extensions;
using DataStreaming.Models.FileTransfer;
using DataStreaming.Protocols;
using DataStreaming.Protocols.Factories;
using DataStreaming.Protocols.Interfaces;
using DataStreaming.Services.Interfaces;
using DataStreaming.Settings;

namespace DataStreaming.Services.FileTransfer;

public class RetranslationServer : IRetranslationServer, INetworkService<FileRetranslationSettings>,
    INotifyListeningStarted
{
    private CancellationTokenSource _cts;

    public RetranslationServer(FileRetranslationSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public FileRetranslationSettings Settings { get; }

    public Dictionary<IPEndPoint, ClientProxy> ClientProxies { get; } = new();

    public event AsyncEventHandler<ListeningEventArgs>? ListeningStarted;

    public async Task<bool> Start()
    {
        if (_cts is not null)
            return false;

        _cts = new CancellationTokenSource();
        var protoFactory = FileRetranslationProtocolFactory.Create();

        var endPoint = new IPEndPoint(IPAddress.Any, Settings.Port);
        var listener = new TcpListener(endPoint);
        listener.Start();
        ListeningStarted?.Invoke(this, new ListeningEventArgs(endPoint));

        while (!_cts.Token.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(_cts.Token);
            var clientProxy = CreateClientProxy(client, protoFactory);
            ClientProxies.Add(clientProxy.EndPoint, clientProxy);
            _ = clientProxy.DoCommunication(_cts.Token);
        }

        return true;
    }

    public Task<bool> Stop()
    {
        if (_cts is null)
            return Task.FromResult(false);

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;

        return Task.FromResult(true);
    }

    private ClientProxy CreateClientProxy(TcpClient client, IProtocolFactory factory)
    {
        var proto = (RetranslationServerProto)factory.CreateServerProtocol();
        proto.RetranslationSettings = Settings;
        proto.FileUploaded += OnImageUploaded;

        var ep = client.GetRemoteEndpoint()!;
        var clientProxy = new ClientProxy(ep, proto);
        clientProxy.SetClient(client);
        return clientProxy;
    }

    private async void OnImageUploaded(object? sender, FileUploadedEventArgs e)
    {
        var dataLengthBytes = ((long)e.FileData.Length).ToNetworkBytes();
        var nameLengthBytes = e.FileNameData.Length.ToNetworkBytes();
        var addressBytes = e.Uploader.Address.GetAddressBytes();
        var portBytes = e.Uploader.Port.ToNetworkBytes();

        var mNumber = e.MessageOrderNumber;
        var bSize = e.BatchSize;

        var tasks = ClientProxies.Values
            .Where(p => p.ClientType == ClientType.Receiver)
            .Select(async receiver =>
            {
                var stream = receiver.Client.GetStream();
                stream.Write(nameLengthBytes);
                stream.Write(e.FileNameData);
                stream.Write(dataLengthBytes);
                await stream.WriteAsync(e.FileData);
                stream.Write(addressBytes);
                stream.Write(portBytes);
                if (mNumber == bSize)
                    stream.Write(Prologs.EndOfBatch.ToNetworkBytes());
                await stream.FlushAsync();
            });

        try
        {
            await tasks.WhenAll();
            Console.WriteLine($"[RetranslationServer]: Sent file '{Encoding.UTF8.GetString(e.FileNameData)}' to all");
        }
        catch (AggregateException exception)
        {
            Console.WriteLine(exception);
        }
    }
}