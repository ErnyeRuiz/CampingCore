namespace CampingCore.Infrastructure.Storage;

public sealed class R2StorageSettings
{
    public const string SectionName = "CloudflareR2";

    public string AccountId { get; init; }     = string.Empty;
    public string AccessKeyId { get; init; }   = string.Empty;
    public string SecretAccessKey { get; init; } = string.Empty;
    public string BucketName { get; init; }    = string.Empty;
    public string PublicBaseUrl { get; init; } = string.Empty;
}
