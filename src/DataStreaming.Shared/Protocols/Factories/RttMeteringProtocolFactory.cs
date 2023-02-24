using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Protocols.Interfaces;
using DataStreaming.Protocols.Interfaces.RTT;
using DataStreaming.Protocols.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Factories;

public class RttMeteringProtocolFactory : IRttMeteringProtocolFactory
{
    private RttMeteringProtocolFactory()
    {
    }

    public IClientSocketProtocol CreateClientProtocol() =>
        throw new NotImplementedException(
            $"Use CreateClientProtocol with {nameof(RttMeteringSettings)} parameter instead");

    public IServerSocketProtocol CreateServerProtocol() => new RttMeteringServerProto();

    public static ISocketProtocolFactory Create() => new RttMeteringProtocolFactory();

    public IRttMeteringProtocol CreateClientProtocol(RttMeteringSettings settings)
    {
        IRttMeteringHandler handler;

        switch (settings.MeteringType)
        {
                case RttMeteringType.AggregationInterval:
                    throw new NotImplementedException();
                    ThrowIfInvalidSettings(settings.AggregationInterval, RttMeteringType.AggregationInterval);
                case RttMeteringType.SinglePacket:
                    ThrowIfInvalidSettings(settings.SinglePacket, RttMeteringType.SinglePacket);
                    handler = new SinglePacketHandler(settings.SinglePacket);
                    break;
                default: throw new InvalidOperationException();
        }

        return new RttMeteringClientProto(handler, settings);
    }

    private void ThrowIfInvalidSettings(HandlerMeteringSettings settings, RttMeteringType meteringType)
    {
        // 4(packet length) + 8(packet number, ulong)
        if (settings.PacketSize < 12 || settings.PacketSize > 64495)
            throw new ArgumentException($"{nameof(settings.PacketSize)}  is constrained with 12-64495 inclusive range");
        
        if (meteringType == RttMeteringType.SinglePacket)
            if (settings.Interval < 20 || settings.Interval > 5000)
                throw new ArgumentException(
                    $"Interval in {RttMeteringType.SinglePacket} mode is constrained with 20-5000ms inclusive range");
        
        if (meteringType == RttMeteringType.AggregationInterval)
            if (settings.Interval < 200 || settings.Interval > 5000)
                throw new ArgumentException($"Interval in {RttMeteringType.AggregationInterval} mode is constrained with 200-5000ms inclusive range");
    }
}