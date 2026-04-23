using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface ICampSiteRepository
{
    Task<CampSite?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampSite>> GetAllAsync(CancellationToken cancellationToken = default);
    Task RecalculateCampSiteRatingAsync(int campSiteId, CancellationToken cancellationToken = default);

    void Add(CampSite campSite);
    void Remove(CampSite campSite);
}
