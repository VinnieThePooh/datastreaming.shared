using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Factories;

public interface ISocketProtocolFactory
{
    IClientSocketProtocol CreateClientProtocol();

    IServerSocketProtocol CreateServerProtocol();

    static abstract ISocketProtocolFactory Create();
}