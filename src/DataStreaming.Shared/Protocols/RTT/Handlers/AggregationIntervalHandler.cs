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
    private int sendCounter = 0;
    private int receiveCounter = 0;

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

        ReceivingTask = Task.Factory.StartNew(() => StartReceiving(party, token), TaskCreationOptions.AttachedToParent);
        while (!token.IsCancellationRequested)
        {
            //don't wait here - we need to stop sending first
            RunWithTimer(period, SendPart, party, token, HandlingTaskType.Sending);
            barrierObject.SignalAndWait(token);
        }
        token.ThrowIfCancellationRequested();
    }

    protected override async Task StartReceiving(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        while (!token.IsCancellationRequested)
        {
            //wait here
            await RunWithTimer(period, ReceivePart, party, token, HandlingTaskType.Receiving);
            barrierObject.SignalAndWait(token);
        }
        token.ThrowIfCancellationRequested();
    }

    async Task RunWithTimer(TimeSpan period, Func<Socket, CancellationToken, Task> function, Socket socket, CancellationToken token, HandlingTaskType taskType)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(period);
        try
        {
            await Task.Run(() => function(socket, cts.Token), cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine($"[{taskType}]: task was cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ReceivePart(Socket party, CancellationToken token)
    {
        //fine-grained token - don't throw OCE here
        receiveCounter = 0;
        while (!token.IsCancellationRequested)
        {
            streamInfo = await ReadStreamData(party, token, streamInfo);
            if (streamInfo.IsDisconnectedPrematurely)
                throw new DisconnectedPrematurelyException();

            receiveCounter++;
            var message = streamInfo.Message;
            if (_statsMap.TryRemove(message.SequenceNumber, out var stats))
            {
                stats.RttValue = Stopwatch.GetElapsedTime(stats.SendTimeTrace, message.Timetrace);
                notifyBuffer.Add(stats);
            }
        }

        Debug.WriteLine($"[{HandlingTaskType.Receiving}]: {receiveCounter} packets received");
    }

    private async Task SendPart(Socket party, CancellationToken token)
    {
        //fine-grained token - don't throw OCE here
        sendCounter = 0;
        while (!token.IsCancellationRequested)
        {
            var stats = RttStats.WithCurrentTimetrace(messageCounter);
            await party.SendAsync(memory, token);
            _statsMap.TryAdd(messageCounter, stats);
            IncrementCounter(memory.Span, ++messageCounter);
            sendCounter++;
        }

        Debug.WriteLine($"[{HandlingTaskType.Sending}]: {sendCounter} packets sent");
    }

    private void OnPostPhaseAction(Barrier barrier)
    {
        Debug.WriteLine($"NotifyBuffer.Count: {notifyBuffer.Count}  (for {_settings.Interval}ms)");
        if (notifyBuffer.Count is 0)
        {
            Debug.WriteLine("[Notify]: NotifyBuffer.Count is 0. Probably receiving Task was cancelled. Skipping...");
            return;
        }
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