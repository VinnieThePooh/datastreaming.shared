using DataStreaming.Events;
using DataStreaming.Settings;

namespace DataStreaming.Services;

public interface INetworkService<out TSettings> where TSettings : HostSettings
{
    Task<bool> Start();

    Task<bool> Stop();

    TSettings Settings { get; }
}