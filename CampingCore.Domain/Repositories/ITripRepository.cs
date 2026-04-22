using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trip>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    void Add(Trip trip);
    void Remove(Trip trip);
}
