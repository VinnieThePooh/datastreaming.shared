using System.Net.Sockets;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class SinglePacketHandler : RttMeteringHandlerBase
{
    public SinglePacketHandler(HandlerMeteringSettings settings) : base(settings)
    {
    }

    public override async Task DoCommunication(Socket party, CancellationToken token)
    {
        ulong counter = 1;
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_settings.Interval));
        var memory = InitMemory(_settings.PacketSize, counter);
        ReceivingTask = Task.Run(() => StartReceiving(party, token), token);
        do
        {
            var stats = RttStats.WithCurrentTimetrace(counter);
            await party.SendAsync(memory, token);
            _statsMap.TryAdd(counter, stats);
            IncrementCounter(memory.Span, ++counter);
        } while (await timer.WaitForNextTickAsync(token));
    }
}