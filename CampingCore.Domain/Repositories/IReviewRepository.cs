using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByCampSiteIdAsync(int campSiteId, CancellationToken cancellationToken = default);
    void Add(Review review);
    void Remove(Review review);
}
