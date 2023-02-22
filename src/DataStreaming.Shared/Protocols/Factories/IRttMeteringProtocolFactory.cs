using DataStreaming.Protocols.Interfaces;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Factories;

public interface IRttMeteringProtocolFactory : ISocketProtocolFactory
{
     IClientSocketProtocol CreateClientProtocol(RttMeteringSettings settings);
}