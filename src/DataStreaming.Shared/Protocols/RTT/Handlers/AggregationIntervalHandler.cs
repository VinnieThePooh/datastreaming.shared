using System.Buffers;
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
    private RttStreamingInfo streamInfo;
    private Memory<byte> memory;
    private ulong messageCounter = 0;
    private int sendCounter = 0;
    private int receiveCounter = 0;
    private CancellationTokenSource criticalCancellationSource;

    public AggregationIntervalHandler(HandlerMeteringSettings settings) : base(settings)
    {
        streamInfo = RttStreamingInfo.InitWithPacketSize(_settings.PacketSize);
        MeteringType = RttMeteringType.AggregationInterval;
        barrierObject = new Barrier(2, OnPostPhaseAction);
    }

    public override async Task DoCommunication(Socket party, CancellationToken token)
    {
        criticalCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        var criticalToken = criticalCancellationSource.Token;
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        memory = InitMemory(_settings.PacketSize, messageCounter);

        ReceivingTask = Task.Factory.StartNew(() => StartReceiving(party, criticalToken),
            TaskCreationOptions.AttachedToParent);
        while (!criticalToken.IsCancellationRequested)
        {
            //don't wait here - we need to stop sending first
            var t = RunWithTimer(period, SendPart, party, criticalToken, HandlingTaskType.Sending);
            barrierObject.SignalAndWait(criticalToken);
        }
        criticalToken.ThrowIfCancellationRequested();
    }

    protected override async Task StartReceiving(Socket party, CancellationToken token)
    {
        var period = TimeSpan.FromMilliseconds(_settings.Interval);
        while (!token.IsCancellationRequested)
        {
            //wait here
            var effective = await ReceivePart(party, token);
            if (!effective)
                continue;
            barrierObject.SignalAndWait(token);
        }

        token.ThrowIfCancellationRequested();
    }

    async Task RunWithTimer(TimeSpan period, Func<Socket, CancellationToken, Task> function, Socket socket,
        CancellationToken token, HandlingTaskType taskType)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(period);
        try
        {
            await Task.Run(() => function(socket, cts.Token), cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine($"[{taskType}-{barrierObject.CurrentPhaseNumber}]: sending period timed out");
        }
        catch (SocketException se)
        {
            if (se.ErrorCode is 104)
            {
                Debug.WriteLine($"[Critical-{barrierObject.CurrentPhaseNumber}]: Metering server reseted connection...Cancelling work");
                criticalCancellationSource.Cancel();
            }
        }
        catch (Exception e)
        {
            //critical
            var pn = barrierObject.CurrentPhaseNumber;
            Debug.WriteLine($"[Critical-{pn}]: Unexpected exception: {e}");
            Debug.WriteLine($"[Critical-{pn}]: Cancelling work");
            criticalCancellationSource.Cancel();
        }
    }

    private async Task<bool> ReceivePart(Socket party, CancellationToken token)
    {
        receiveCounter = 0;
        while (!_statsMap.IsEmpty && !token.IsCancellationRequested)
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

        Debug.WriteLine($"[{HandlingTaskType.Receiving}-{barrierObject.CurrentPhaseNumber}]: {receiveCounter} packets received");
        return receiveCounter > 0;
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
        Debug.WriteLine($"[{HandlingTaskType.Sending}-{barrierObject.CurrentPhaseNumber}]: {sendCounter} packets sent");
    }

    private void OnPostPhaseAction(Barrier barrier)
    {
        Debug.WriteLine(
            $"[Notify-{barrier.CurrentPhaseNumber}]: NotifyBuffer.Count is {notifyBuffer.Count}  (for {_settings.Interval}ms)");
        if (notifyBuffer.Count is 0)
        {
            Debug.WriteLine(
                $"[Notify-{barrier.CurrentPhaseNumber}]: NotifyBuffer.Count is 0. Probably receiving Task was cancelled. Skipping...");
            return;
        }

        var stats = new AggregatedRttStats
        {
            AggregationInterval = _settings.Interval,
            PacketsCount = notifyBuffer.Count,
            SequenceNumber = notifyBuffer.Min(x => x.SequenceNumber),
            PhaseNumber = barrierObject.CurrentPhaseNumber,
            AvgRtt = TimeSpan.FromTicks((long)notifyBuffer.Average(x => x.RttValue.Ticks)),
            MinRtt = notifyBuffer.Min(x => x.RttValue),
            MaxRtt = notifyBuffer.Max(x => x.RttValue)
        };
        InvokeStatsEvent(this, new RttStatisticsEventArgs(stats.SequenceNumber, stats));
        notifyBuffer.Clear();
    }
}