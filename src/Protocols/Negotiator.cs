﻿using System.Net.Sockets;
using DataStreaming.Services;
using ImageRetranslationShared.Models.Negotiation;

namespace DataStreaming.Common.Protocols;

public class Negotiator : IProtocolNegotiator
{
    public Negotiator(TcpClient party)
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