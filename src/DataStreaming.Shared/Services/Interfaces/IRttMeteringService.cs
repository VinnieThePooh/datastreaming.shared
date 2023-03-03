using DataStreaming.Constants.RTT;
using DataStreaming.Events.RTT;
using DataStreaming.Protocols.Handlers.RTT;
using DataStreaming.Settings;

namespace DataStreaming.Services.Interfaces;

public interface IRttMeteringService : INetworkService<RttMeteringSettings>, INotifyRttStatistics, IAsyncDisposable
{
    RttMeteringType MeteringType { get; }

    IRttMeteringHandler MeteringHandler { get; }

    RttMeteringSettings MeteringSettings { get; }
}