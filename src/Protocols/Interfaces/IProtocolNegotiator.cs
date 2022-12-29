using System.Net.Sockets;
using ImageRetranslationShared.Models.Negotiation;

namespace DataStreaming.Services;

public interface IProtocolNegotiator : IAsyncDisposable
{
    TcpClient Party { get; }
    Task<NegotiationResult> SendOptions(NegotiationOption[] options, CancellationToken token);

    Task SendChosenOption(ChosenNegotiationOption option, CancellationToken token);
}