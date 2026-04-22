using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface IFavoriteRepository
{
    Task<Favorite?> GetByUserAndCampSiteAsync(int userId, int campSiteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    void Add(Favorite favorite);
    void Remove(Favorite favorite);
}
