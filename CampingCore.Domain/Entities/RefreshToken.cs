using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

/// <summary>
/// Sesión de renovación opaca (refresh). El valor enviado al cliente no se guarda; solo su hash.
/// </summary>
public sealed class RefreshToken : Entity<Guid>
{
    public int UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastUsedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public User? User { get; private set; }

    private RefreshToken() : base(default) { }

    private RefreshToken(Guid id, int userId, string tokenHash, DateTime utcNow)
        : base(id)
    {
        UserId        = userId;
        TokenHash     = tokenHash;
        CreatedAtUtc  = utcNow;
        LastUsedAtUtc = utcNow;
    }

    public static RefreshToken Create(int userId, string tokenHash, DateTime utcNow)
        => new(Guid.NewGuid(), userId, tokenHash, utcNow);

    public void Revoke(DateTime utcNow) => RevokedAtUtc = utcNow;
}
