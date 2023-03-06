using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Protocols.Interfaces.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.RTT;

public class RttMeteringClientProto : IRttMeteringProtocol
{
    public RttMeteringClientProto(IRttMeteringHandler handler, RttMeteringSettings settings)
    {
        MeteringHandler = handler;
        MeteringSettings = settings;
    }

    public Task DoCommunication(Socket party, CancellationToken token) =>
        Task.Run(() => MeteringHandler.DoCommunication(party, token), token);

    public RttMeteringType MeteringType => MeteringHandler.MeteringType;

    public IRttMeteringHandler MeteringHandler { get; }

    public RttMeteringSettings MeteringSettings { get; }
}