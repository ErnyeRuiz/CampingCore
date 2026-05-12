using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context) => _context = context;

    public void Add(RefreshToken token) => _context.RefreshTokens.Add(token);

    public Task<RefreshToken?> GetActiveByHashWithUserAsync(string tokenHash, CancellationToken cancellationToken = default)
        => _context.RefreshTokens
            .Include(t => t.User!)
                .ThenInclude(u => u!.Role!)
                    .ThenInclude(r => r!.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash && t.RevokedAtUtc == null,
                cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
        => await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
}
