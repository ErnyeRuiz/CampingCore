using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository : Repository<Role, int>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken cancellationToken = default)
        => await Context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public new async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Roles.AnyAsync(r => r.Name == name, cancellationToken);
}
