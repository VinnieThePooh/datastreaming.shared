using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Factories;

public interface IProtocolFactory
{
    IClientProtocol CreateClientProtocol();

    IServerProtocol CreateServerProtocol();

    static abstract IProtocolFactory Create();
}