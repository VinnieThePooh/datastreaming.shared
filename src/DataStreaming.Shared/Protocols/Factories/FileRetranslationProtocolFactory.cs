using DataStreaming.Protocols.Interfaces;
using DataStreaming.Services.Interfaces;

namespace DataStreaming.Protocols.Factories;

//todo: may be implement some tricky IClientProtocol later
public class FileRetranslationProtocolFactory : IProtocolFactory
{
    private FileRetranslationProtocolFactory()
    {
    }

    public IClientProtocol CreateClientProtocol() => throw new NotSupportedException($"Not supported: Use {nameof(IFileSender)} or {nameof(IFileReceiver)} implementations instead");

    public IServerProtocol CreateServerProtocol() => new RetranslationServerProto();

    public static IProtocolFactory Create() => new FileRetranslationProtocolFactory();
}