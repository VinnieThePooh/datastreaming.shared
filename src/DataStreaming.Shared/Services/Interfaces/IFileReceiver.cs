using DataStreaming.Common.Events;
using DataStreaming.Models;

namespace DataStreaming.Services.Interfaces;

public interface IFileReceiver : IAsyncDisposable
{
    event EventHandler<BatchLoadedEventArgs> BatchLoaded;
    IAsyncEnumerable<NetworkFile> AwaitFiles(CancellationToken token);
}