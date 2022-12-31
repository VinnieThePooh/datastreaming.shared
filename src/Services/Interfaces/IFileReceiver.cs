using DataStreaming.Common.Events;
using DataStreaming.Models;

namespace DataStreaming.Services;

public interface IFileReceiver : IAsyncDisposable
{
    event EventHandler<BatchLoadedEventArgs> BatchLoaded;
    IAsyncEnumerable<NetworkFile> AwaitFiles(CancellationToken token);
}