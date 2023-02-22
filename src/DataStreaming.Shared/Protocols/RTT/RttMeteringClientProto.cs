using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Protocols.Interfaces;
using DataStreaming.Protocols.Interfaces.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringClientProto : IRttMeteringProtocol, IClientSocketProtocol
{
    public RttMeteringClientProto(IRttMeteringHandler handler, RttMeteringSettings settings)
    {
        MeteringHandler = handler;
        Settings = settings;
    }

    public Task DoCommunication(Socket party, CancellationToken token)
    {
        return Task.Run(() => MeteringHandler.DoCommunication(party, token), token);
    }

    public RttMeteringType MeteringType => MeteringHandler.MeteringType;

    public IRttMeteringHandler MeteringHandler { get; }

    public RttMeteringSettings Settings { get; }
}