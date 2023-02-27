using DataStreaming.Constants.RTT;
using DataStreaming.Protocols.Interfaces;
using DataStreaming.Protocols.RTT.Handlers;
using DataStreaming.Settings;

namespace DataStreaming.Services.Interfaces;

public interface IRttMeteringService : INetworkService<RttMeteringSettings>, INotifyRttStatistics, IAsyncDisposable
{
    RttMeteringType MeteringType { get; }

    IRttMeteringHandler MeteringHandler { get; }

    RttMeteringSettings MeteringSettings { get; }
}