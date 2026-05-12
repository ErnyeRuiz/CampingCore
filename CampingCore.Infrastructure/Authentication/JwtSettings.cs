namespace CampingCore.Infrastructure.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey     { get; init; } = string.Empty;
    public string Issuer        { get; init; } = string.Empty;
    public string Audience      { get; init; } = string.Empty;
    public int    ExpiryMinutes { get; init; } = 60;

    /// <summary>Días sin renovación antes de invalidar la sesión.</summary>
    public int RefreshIdleTimeoutDays { get; init; } = 30;
}
