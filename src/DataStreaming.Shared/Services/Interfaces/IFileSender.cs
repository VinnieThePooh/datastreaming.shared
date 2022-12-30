namespace ImageRetranslationShared.Services.Interfaces;

public interface IFileSender : IAsyncDisposable
{
    Task SendFiles(IEnumerable<string> filePaths, CancellationToken token);
}