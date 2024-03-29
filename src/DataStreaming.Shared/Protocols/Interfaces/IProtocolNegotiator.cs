﻿using System.Net.Sockets;
using DataStreaming.Models.Negotiation;

namespace DataStreaming.Protocols.Interfaces;

public interface IProtocolNegotiator : IAsyncDisposable
{
    TcpClient Party { get; }

    Task<NegotiationResult> SendOptions(NegotiationOption[] options, CancellationToken token);

    Task SendChosenOption(ChosenNegotiationOption option, CancellationToken token);
}