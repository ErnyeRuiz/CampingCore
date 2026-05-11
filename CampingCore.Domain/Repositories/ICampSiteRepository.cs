using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface ICampSiteRepository
{
    Task<CampSite?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampSite>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampSite>> GetByCreatedByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task RecalculateCampSiteRatingAsync(int campSiteId, CancellationToken cancellationToken = default);

    Task<(int TotalCount, decimal? AverageRating)> GetStatsAsync(
        int? idProvincia,
        int? idCanton,
        int? idDistrito,
        CancellationToken cancellationToken = default);

    void Add(CampSite campSite);
    void Remove(CampSite campSite);
}
