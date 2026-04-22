using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class FavoriteRepository : Repository<Favorite, int>, IFavoriteRepository
{
    public FavoriteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Favorite?> GetByUserAndCampSiteAsync(int userId, int campSiteId, CancellationToken cancellationToken = default)
        => await Context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CampSiteId == campSiteId, cancellationToken);

    public async Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await Context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync(cancellationToken);
}
