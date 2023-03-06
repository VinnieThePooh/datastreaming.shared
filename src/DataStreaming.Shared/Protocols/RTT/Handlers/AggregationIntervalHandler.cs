using System.Diagnostics;
using System.Net.Sockets;
using System.Security;
using DataStreaming.Constants.RTT;
using DataStreaming.Events.RTT;
using DataStreaming.Exceptions;
using DataStreaming.Models.Common;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class AggregationIntervalHandler : RttMeteringHandlerBase
{
    private readonly Barrier barrierObject;
    private readonly List<RttStats> notifyBuffer = new();
    private RttStreamingInfo streamInfo;
    private Memory<byte> memory;
    private ulong messageCounter = 0;

    public AggregationIntervalHandler(HandlerMeteringSettings settings) : base(settings)
    {
        streamInfo = RttStreamingInfo.InitWithPacketSize(_settings.PacketSize);
        MeteringType = RttMeteringType.AggregationInterval;
        barrierObject = new Barrier(2, OnPostPhaseAction);
    }

    public override async Task DoCommunication(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        memory = InitMemory(_settings.PacketSize, messageCounter);
        
        ReceivingTask = Task.Run(() => StartReceiving(party, token), token);
        while (!token.IsCancellationRequested)
        {
            RunWithTimer(period, SendPart, party, token);
            barrierObject.SignalAndWait(token);
        }
        token.ThrowIfCancellationRequested();
    }

    protected override async Task StartReceiving(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        while (!token.IsCancellationRequested)
        {
            RunWithTimer(period, ReceivePart, party, token);
            barrierObject.SignalAndWait(token);
        }
        token.ThrowIfCancellationRequested();
    }

    void RunWithTimer(TimeSpan period, Func<Socket, CancellationToken, Task> function, Socket socket, CancellationToken token)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(period);
        //redundant check after method return?
        while (!cts.Token.IsCancellationRequested)
            function(socket, cts.Token);
    }

    private async Task ReceivePart(Socket party, CancellationToken token)
    {
        //fine-grained token
        while (!token.IsCancellationRequested)
        {
            streamInfo = await ReadStreamData(party, token, streamInfo);
            if (streamInfo.IsDisconnectedPrematurely)
                throw new DisconnectedPrematurelyException();

            var message = streamInfo.Message;
            if (_statsMap.TryRemove(message.SequenceNumber, out var stats))
            {
                stats.RttValue = Stopwatch.GetElapsedTime(stats.SendTimeTrace, message.Timetrace);
                notifyBuffer.Add(stats);
            }
        }
    }

    private async Task SendPart(Socket party, CancellationToken token)
    {
        //fine-grained token
        while (!token.IsCancellationRequested)
        {
            var stats = RttStats.WithCurrentTimetrace(messageCounter);
            await party.SendAsync(memory, token);
            _statsMap.TryAdd(messageCounter, stats);
            IncrementCounter(memory.Span, ++messageCounter);
        }
    }

    private void OnPostPhaseAction(Barrier barrier)
    {
        var stats = new AggregatedRttStats
        {
            AggregationInterval = _settings.Interval,
            PacketsCount = notifyBuffer.Count,
            SequenceNumber = notifyBuffer.Min(x => x.SequenceNumber),
            AvgRtt = TimeSpan.FromTicks((long)notifyBuffer.Average(x => x.RttValue.Ticks)),
            MinRtt = notifyBuffer.Min(x => x.RttValue),
            MaxRtt = notifyBuffer.Max(x => x.RttValue)
        };
        InvokeStatsEvent(this, new RttStatisticsEventArgs(stats.SequenceNumber, stats));
        notifyBuffer.Clear();
    }
}