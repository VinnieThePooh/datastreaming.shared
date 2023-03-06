using System.Diagnostics;
using System.Net.Sockets;
using DataStreaming.Events.RTT;
using DataStreaming.Exceptions;
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

    async Task StartReceiving(Socket party, CancellationToken token)
    {
        var streamInfo = RttStreamingInfo.InitWithPacketSize(_settings.PacketSize);

        while (!token.IsCancellationRequested)
        {
            streamInfo = await ReadStreamData(party, token, streamInfo);
            if (streamInfo.IsDisconnectedPrematurely)
                throw new DisconnectedPrematurelyException();

            var message = streamInfo.Message;
            if (_statsMap.TryRemove(message.SequenceNumber, out var stats))
            {
                stats.RttValue = Stopwatch.GetElapsedTime(stats.SendTimeTrace, message.Timetrace);
                InvokeReceivedStatsEvent(this, new RttStatisticsEventArgs(stats.SequenceNumber, stats));
            }
        }

        token.ThrowIfCancellationRequested();
    }
}