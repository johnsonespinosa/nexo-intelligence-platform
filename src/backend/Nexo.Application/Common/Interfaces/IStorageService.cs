namespace Nexo.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(string key, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    string GetPublicUrl(string key);
}
