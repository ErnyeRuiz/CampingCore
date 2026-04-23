using CampingCore.Domain.Entities;
using CampingCore.Domain.Primitives;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class CampSiteRepository : Repository<CampSite, int>, ICampSiteRepository
{
    public CampSiteRepository(ApplicationDbContext context) : base(context) { }

    public async Task RecalculateCampSiteRatingAsync(int campSiteId, CancellationToken cancellationToken = default)
    {
        var avg = await Context.Set<Review>()
            .Where(r => r.CampSiteId == campSiteId)
            .AverageAsync(r => (decimal?)r.Rating, cancellationToken) ?? 0m;

        await Context.Set<CampSite>()
            .Where(c => c.Id == campSiteId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(c => c.Rating, Math.Round(avg, 2)),
                cancellationToken);
    }
}
