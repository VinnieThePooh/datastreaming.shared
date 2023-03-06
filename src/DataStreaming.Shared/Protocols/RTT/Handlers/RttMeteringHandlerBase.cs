using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using DataStreaming.Constants.RTT;
using DataStreaming.Events;
using DataStreaming.Events.RTT;
using DataStreaming.Exceptions;
using DataStreaming.Extensions;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public abstract class RttMeteringHandlerBase : IRttMeteringHandler
{
    protected readonly HandlerMeteringSettings _settings;

    protected RttMeteringHandlerBase(HandlerMeteringSettings settings)
    {
        _settings = settings;
    }

    protected readonly ConcurrentDictionary<ulong, RttStats> _statsMap = new();
    public abstract Task DoCommunication(Socket party, CancellationToken token);

    public event AsyncEventHandler<RttStatisticsEventArgs>? RttReceived;
    public virtual RttMeteringType MeteringType => RttMeteringType.SinglePacket;

    public Task ReceivingTask { get; protected set; }

    protected Memory<byte> InitMemory(int packetSize, ulong counter)
    {
        var memory = new Memory<byte>(new byte[packetSize]);
        packetSize.ToNetworkBytes().CopyTo(memory.Span);
        unchecked((long)counter).ToNetworkBytes().CopyTo(memory.Span.Slice(4, 8));
        return memory;
    }

    protected void IncrementCounter(Span<byte> memory, ulong counter)
    {
        unchecked((long)counter).ToNetworkBytes().CopyTo(memory.Slice(4, 8));
    }

    protected async ValueTask<RttStreamingInfo> ReadStreamData(Socket party, CancellationToken token,
        RttStreamingInfo streamingInfo)
    {
        var pSize = (int)streamingInfo.PacketSize!;
        var totalRead = streamingInfo.LeftData.Length;
        var leftToRead = pSize - totalRead;
        var toWrite = 0;
        var read = 0;

        var leftData = streamingInfo.LeftData;
        if (!leftData.IsEmpty)
        {
            leftData.CopyTo(streamingInfo.MessageBuffer);
            streamingInfo.LeftData = Memory<byte>.Empty;
        }

        while (totalRead < pSize)
        {
            read = await party.ReceiveAsync(streamingInfo.MessageBuffer[totalRead..], token);
            toWrite = Math.Min(read, leftToRead);
            leftToRead -= toWrite;
            totalRead += read;
        }

        streamingInfo.MessageTimetrace = Stopwatch.GetTimestamp();

        if (toWrite < read)
        {
            var delta = read - toWrite;
            var from = totalRead - read + toWrite;
            streamingInfo.LeftData = streamingInfo.MessageBuffer[from..(from + delta)];
        }

        streamingInfo.ConstructMessage();
        return streamingInfo;
    }
    
    protected virtual async Task StartReceiving(Socket party, CancellationToken token)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void InvokeReceivedStatsEvent(RttMeteringHandlerBase sender, RttStatisticsEventArgs eventArgs) =>
        RttReceived?.Invoke(sender, eventArgs);
}