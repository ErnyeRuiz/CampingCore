using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class PermissionRepository : Repository<Permission, int>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context) { }

    public new async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Permissions.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        => await Context.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
}
