using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);

    Task<RefreshToken?> GetActiveByHashWithUserAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
