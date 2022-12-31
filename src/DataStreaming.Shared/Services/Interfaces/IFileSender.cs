namespace DataStreaming.Services.Interfaces;

public interface IFileSender : IAsyncDisposable
{
    Task SendFiles(IEnumerable<string> filePaths, CancellationToken token);
}