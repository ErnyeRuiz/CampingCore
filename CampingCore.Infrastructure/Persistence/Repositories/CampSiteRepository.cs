using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class CampSiteRepository : Repository<CampSite, int>, ICampSiteRepository
{
    public CampSiteRepository(ApplicationDbContext context) : base(context) { }

    public new async Task<CampSite?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Context.Set<CampSite>()
            .AsSplitQuery()
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public new async Task<IReadOnlyList<CampSite>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<CampSite>()
            .AsSplitQuery()
            .Include(c => c.Images)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CampSite>> GetByCreatedByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await Context.Set<CampSite>()
            .AsSplitQuery()
            .Include(c => c.Images)
            .Where(c => c.CreatedByUserId == userId)
            .ToListAsync(cancellationToken);

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

    public async Task<(int TotalCount, decimal? AverageRating)> GetStatsAsync(
        int? idProvincia,
        int? idCanton,
        int? idDistrito,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<CampSite>().AsQueryable();

        if (idProvincia is { } p)
            query = query.Where(c => c.IdProvincia == p);
        if (idCanton is { } ca)
            query = query.Where(c => c.IdCanton == ca);
        if (idDistrito is { } d)
            query = query.Where(c => c.IdDistrito == d);

        var totalCount = await query.CountAsync(cancellationToken);
        var avg = await query
            .Where(c => c.Rating > 0)
            .AverageAsync(c => (decimal?)c.Rating, cancellationToken);

        decimal? averageRating = avg.HasValue ? Math.Round(avg.Value, 2) : null;

        return (totalCount, averageRating);
    }
}
