using CampingCore.Domain.Entities;

namespace CampingCore.Domain.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    void Add(Permission permission);
    void Remove(Permission permission);
}
