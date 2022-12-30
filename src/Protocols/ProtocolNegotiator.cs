﻿using System.Net.Sockets;
using ImageRetranslationShared.Models.Negotiation;
using ImageRetranslationShared.Protocols.Interfaces;

namespace ImageRetranslationShared.Protocols;

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