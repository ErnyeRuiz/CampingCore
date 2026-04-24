using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class TripRepository : Repository<Trip, int>, ITripRepository
{
    public TripRepository(ApplicationDbContext context) : base(context) { }

    public new async Task<Trip?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await WithTripCampSiteSummaries(Context.Trips)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await WithTripCampSiteSummaries(Context.Trips)
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);

    private static IQueryable<Trip> WithTripCampSiteSummaries(IQueryable<Trip> trips) =>
        trips
            .AsSplitQuery()
            .Include(t => t.CampSites)
            .ThenInclude(tc => tc.CampSite)
            .ThenInclude(c => c!.Images.OrderBy(i => i.Id).Take(1));
}
