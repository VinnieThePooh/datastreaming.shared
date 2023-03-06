using System.Diagnostics;
using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Events.RTT;
using DataStreaming.Exceptions;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class AggregationIntervalHandler : RttMeteringHandlerBase
{
    private readonly Barrier barrierObject;
    private readonly List<RttStats> notifyBuffer = new();

    public AggregationIntervalHandler(HandlerMeteringSettings settings) : base(settings)
    {
        MeteringType = RttMeteringType.AggregationInterval;
        barrierObject = new Barrier(2, OnPostPhaseAction);
    }

    public override async Task DoCommunication(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        ulong counter = 1;

        var memory = InitMemory(_settings.PacketSize, counter);
        ReceivingTask = Task.Run(() => StartReceiving(party, token), token);
        while (!token.IsCancellationRequested)
        {
            RunWithTimer(period, async (s, t) =>
            {
                //fine-grained token here
                while (!t.IsCancellationRequested)
                {
                    var stats = RttStats.WithCurrentTimetrace(counter);
                    await party.SendAsync(memory, token);
                    _statsMap.TryAdd(counter, stats);
                    IncrementCounter(memory.Span, ++counter);
                }
            }, party, token);
            barrierObject.SignalAndWait(token);
        }

        token.ThrowIfCancellationRequested();
    }

    protected override async Task StartReceiving(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        var streamInfo = RttStreamingInfo.InitWithPacketSize(_settings.PacketSize);

        while (!token.IsCancellationRequested)
        {
            //fine-grained token
            RunWithTimer(period, async (s, t) =>
            {
                while (!t.IsCancellationRequested)
                {
                    streamInfo = await ReadStreamData(party, t, streamInfo);
                    if (streamInfo.IsDisconnectedPrematurely)
                        throw new DisconnectedPrematurelyException();

                    var message = streamInfo.Message;
                    if (_statsMap.TryRemove(message.SequenceNumber, out var stats))
                    {
                        stats.RttValue = Stopwatch.GetElapsedTime(stats.SendTimeTrace, message.Timetrace);
                        notifyBuffer.Add(stats);
                    }
                }
            }, party, token);
            barrierObject.SignalAndWait(token);
        }
    }

    void RunWithTimer(TimeSpan period, Func<Socket, CancellationToken, Task> function, Socket socket,
        CancellationToken token)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(period);
        //redundant check after method return?
        while (!cts.Token.IsCancellationRequested)
            function(socket, cts.Token);
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