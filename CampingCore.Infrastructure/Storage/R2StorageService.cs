using Amazon.S3;
using Amazon.S3.Model;
using CampingCore.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace CampingCore.Infrastructure.Storage;

internal sealed class R2StorageService : IR2StorageService
{
    private readonly AmazonS3Client _client;
    private readonly string _bucketName;
    private readonly string _publicBaseUrl;

    public R2StorageService(IOptions<R2StorageSettings> options)
    {
        var s = options.Value;
        var config = new AmazonS3Config
        {
            ServiceURL    = $"https://{s.AccountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
        };
        _client        = new AmazonS3Client(s.AccessKeyId, s.SecretAccessKey, config);
        _bucketName    = s.BucketName;
        _publicBaseUrl = s.PublicBaseUrl.TrimEnd('/');
    }

    public async Task<string> UploadAsync(byte[] data, string key, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName            = _bucketName,
            Key                   = key,
            InputStream           = new MemoryStream(data),
            ContentType           = contentType,
            DisablePayloadSigning = true,
        };

        await _client.PutObjectAsync(request, cancellationToken);

        return $"{_publicBaseUrl}/{key}";
    }
}
