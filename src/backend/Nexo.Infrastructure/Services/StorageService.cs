using Microsoft.Extensions.Configuration;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly string _bucketName;
    private readonly string _endpoint;
    private readonly string _accessKey;
    private readonly string _secretKey;

    public StorageService(IConfiguration configuration)
    {
        _bucketName = configuration["Storage:BucketName"] ?? "nexo-documents";
        _endpoint = configuration["Storage:Endpoint"] ?? "localhost:9000";
        _accessKey = configuration["Storage:AccessKey"] ?? "minioadmin";
        _secretKey = configuration["Storage:SecretKey"] ?? "minioadmin";
    }

    // Para desarrollo inicial, usaremos file system local
    private readonly string _localStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Documents");

    public async Task<string> UploadAsync(string key, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
    {
        // Crear directorio si no existe
        var fullPath = Path.Combine(_localStoragePath, key);
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        // Guardar archivo
        await using var fileStreamOutput = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return key;
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_localStoragePath, key);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File {key} not found");

        var stream = new MemoryStream();
        await using var fileStream = new FileStream(fullPath, FileMode.Open);
        await fileStream.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;

        return stream;
    }

    public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_localStoragePath, key);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_localStoragePath, key);
        return Task.FromResult(File.Exists(fullPath));
    }

    public string GetPublicUrl(string key)
    {
        // Para desarrollo local
        return $"/storage/{key}";
    }
}