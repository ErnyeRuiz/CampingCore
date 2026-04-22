using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class TripCampSiteRepository : Repository<TripCampSite, int>, ITripCampSiteRepository
{
    public TripCampSiteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TripCampSite?> GetByTripAndCampSiteAsync(int tripId, int campSiteId, CancellationToken cancellationToken = default)
        => await Context.TripCampSites
            .FirstOrDefaultAsync(tc => tc.TripId == tripId && tc.CampSiteId == campSiteId, cancellationToken);

    public async Task<bool> ExistsAsync(int tripId, int campSiteId, CancellationToken cancellationToken = default)
        => await Context.TripCampSites
            .AnyAsync(tc => tc.TripId == tripId && tc.CampSiteId == campSiteId, cancellationToken);

    public new void Remove(TripCampSite tripCampSite) => Context.TripCampSites.Remove(tripCampSite);
}
