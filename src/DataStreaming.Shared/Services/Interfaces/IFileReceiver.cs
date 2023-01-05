using DataStreaming.Events;
using DataStreaming.Models.FileTransfer;

namespace DataStreaming.Services.Interfaces;

public interface IFileReceiver : IAsyncDisposable
{
    event EventHandler<BatchLoadedEventArgs> BatchLoaded;
    IAsyncEnumerable<NetworkFile> AwaitFiles(CancellationToken token);
}