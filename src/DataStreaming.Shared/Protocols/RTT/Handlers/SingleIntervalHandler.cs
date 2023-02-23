using System.Collections.Concurrent;
using System.Net.Sockets;
using DataStreaming.Constants.RTT;
using DataStreaming.Extensions;
using DataStreaming.Models.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Protocols.Handlers.RTT;

public class SingleIntervalHandler : IRttMeteringHandler
{
    private readonly HandlerMeteringSettings _settings;
    private static readonly ConcurrentDictionary<ulong, RttStats> _statsMap = new();

    public SingleIntervalHandler(HandlerMeteringSettings settings)
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

    Task StartReceiving(Socket party, CancellationToken token)
    {
        var streamInfo = RttStreamingInfo.InitWithPacketSize(_settings.PacketSize);
        
        while (!token.IsCancellationRequested)
        {
            streamInfo = ReadStreamData(party, token, streamInfo);
        }
        

        return Task.CompletedTask;
    }

    private RttStreamingInfo ReadStreamData(Socket party, CancellationToken token, RttStreamingInfo streamingInfo)
    {
        var pSize = streamingInfo.PacketSize;
        var totalRead = streamingInfo.LeftData.Length;
        var read = 0;

        while (totalRead < pSize)
        {
            
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