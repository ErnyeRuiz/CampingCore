namespace CampingCore.Application.Abstractions.Storage;

public interface IR2StorageService
{
    Task<string> UploadAsync(byte[] data, string key, string contentType, CancellationToken cancellationToken = default);
}
