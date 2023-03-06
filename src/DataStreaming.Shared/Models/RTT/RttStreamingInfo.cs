using System.Text;
using DataStreaming.Extensions;
using DataStreaming.Models.Common;

namespace DataStreaming.Models.RTT;

public struct RttStreamingInfo : StreamingInfo<RttResponse>
{
    public Memory<byte> MessageBuffer { get; set; }

    public Memory<byte> LeftData { get; set; }

    public bool IsDisconnectedPrematurely { get; private set; }
    public RttResponse Message { get; private set; }

    public void ConstructMessage(MemoryStream? memory = null)
    {
        //for debug
        var size = MessageBuffer.Span.GetHostOrderInt();
        Message = new RttResponse
        {
            SequenceNumber = unchecked((uint)MessageBuffer.Span[4..].GetHostOrderInt64()),
            Timetrace = MessageTimetrace,
            Text = Encoding.UTF8.GetString(MessageBuffer.Span[12..])
        };
    }

    public int? PacketSize { get; set; }

    // echo message arrived timestamp
    public long MessageTimetrace { get; set; }

    public static RttStreamingInfo InitWithPacketSize(int packetSize)
    {
        return new RttStreamingInfo
        {
            LeftData = Memory<byte>.Empty,
            MessageBuffer = new Memory<byte>(new byte[packetSize]),
            PacketSize = packetSize
        };
    }
}