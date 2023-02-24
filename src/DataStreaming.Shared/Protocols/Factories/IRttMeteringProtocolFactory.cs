using DataStreaming.Protocols.Interfaces.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Factories;

public interface IRttMeteringProtocolFactory : ISocketProtocolFactory
{
     //factory method creating different handlers based on settings
     IRttMeteringProtocol CreateClientProtocol(RttMeteringSettings settings);
}