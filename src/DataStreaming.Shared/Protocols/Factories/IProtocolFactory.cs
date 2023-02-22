namespace DataStreaming.Protocols.Interfaces;

public interface IProtocolFactory
{
    IClientProtocol CreateClientProtocol();

    IServerProtocol CreateServerProtocol();

    static abstract IProtocolFactory Create();
}