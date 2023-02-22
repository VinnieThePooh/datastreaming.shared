using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Protocols.Factories;

public class RttMeteringProtocolFactory : ISocketProtocolFactory
{
    private RttMeteringProtocolFactory()
    {
    }

    public IClientSocketProtocol CreateClientProtocol() => throw new NotImplementedException();

    public IServerSocketProtocol CreateServerProtocol() => throw new NotImplementedException();

    public static ISocketProtocolFactory Create() => new RttMeteringProtocolFactory();
}