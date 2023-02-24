using DataStreaming.Constants.RTT;

namespace DataStreaming.Services.Interfaces;

public interface IRttEchoServer
{
    RttMeteringType MeteringType { get; }
}