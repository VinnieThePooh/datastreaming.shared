using System.Net.Sockets;
using DataStreaming.Models.Negotiation;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols;

public class ProtocolNegotiator : IProtocolNegotiator
{
    public ProtocolNegotiator(TcpClient party)
    {
        Party = party ?? throw new ArgumentNullException(nameof(party));
    }

    public TcpClient Party { get; }

    public Task<NegotiationResult> SendOptions(NegotiationOption[] options, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task SendChosenOption(ChosenNegotiationOption option, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}