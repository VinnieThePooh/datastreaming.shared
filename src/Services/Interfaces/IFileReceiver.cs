using ImageRetranslationShared.Events;
using ImageRetranslationShared.Models.FileTransfer;

namespace ImageRetranslationShared.Services.Interfaces;

public interface IFileReceiver : IAsyncDisposable
{
    event EventHandler<BatchLoadedEventArgs> BatchLoaded;
    IAsyncEnumerable<NetworkFile> AwaitFiles(CancellationToken token);
}