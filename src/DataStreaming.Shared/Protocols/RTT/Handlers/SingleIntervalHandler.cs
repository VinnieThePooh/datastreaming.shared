using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Extensions;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class SingleIntervalHandler : IRttMeteringHandler
{
    private readonly HandlerMeteringSettings _settings;

    public SingleIntervalHandler(HandlerMeteringSettings settings)
    {
        _settings = settings;
    }
    
    public async Task DoCommunication(Socket party, CancellationToken token)
    {
        ulong counter = 1;
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_settings.Interval));
        var memory = InitMemory(_settings.PacketSize, counter);
        do
        {
            await party.SendAsync(memory, token);
            //
            IncrementCounter(memory.Span, ++counter);
        }
        while (await timer.WaitForNextTickAsync(token));
    }

    public RttMeteringType MeteringType => RttMeteringType.SinglePacket;

    private Memory<byte> InitMemory(int packetSize, ulong counter)
    {
        var memory = new Memory<byte>(new byte[packetSize]);
        packetSize.ToNetworkBytes().CopyTo(memory.Span);
        unchecked((long)counter).ToNetworkBytes().CopyTo(memory.Span.Slice(4, 8));
        return memory;
    }

    private void IncrementCounter(Span<byte> memory, ulong counter)
    {
        unchecked((long)counter).ToNetworkBytes().CopyTo(memory.Slice(4, 8));
    }
}