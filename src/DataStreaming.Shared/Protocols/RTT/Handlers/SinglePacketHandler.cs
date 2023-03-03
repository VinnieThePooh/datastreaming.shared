using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Events;
using DataStreaming.Events.RTT;
using DataStreaming.Exceptions;
using DataStreaming.Extensions;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class SinglePacketHandler : IRttMeteringHandler
{
    private readonly HandlerMeteringSettings _settings;
    private static readonly ConcurrentDictionary<ulong, RttStats> _statsMap = new();
    public event AsyncEventHandler<RttStatisticsEventArgs> RttReceived;
    
    public SinglePacketHandler(HandlerMeteringSettings settings)
    {
        _settings = settings;
    }
    
    public async Task DoCommunication(Socket party, CancellationToken token)
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
        }
        while (await timer.WaitForNextTickAsync(token));
    }

    public RttMeteringType MeteringType => RttMeteringType.SinglePacket;
    
    public Task ReceivingTask { get; private set; }

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
                RttReceived?.Invoke(this, new RttStatisticsEventArgs(stats.SequenceNumber, stats));
            }
        }
        token.ThrowIfCancellationRequested();
    }

    private async ValueTask<RttStreamingInfo> ReadStreamData(Socket party, CancellationToken token, RttStreamingInfo streamingInfo)
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