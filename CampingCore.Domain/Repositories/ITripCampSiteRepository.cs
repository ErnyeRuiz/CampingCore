using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface ITripCampSiteRepository
{
    Task<TripCampSite?> GetByTripAndCampSiteAsync(int tripId, int campSiteId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int tripId, int campSiteId, CancellationToken cancellationToken = default);
    void Add(TripCampSite tripCampSite);
    void Remove(TripCampSite tripCampSite);
}
