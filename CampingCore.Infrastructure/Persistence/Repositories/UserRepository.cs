using CampingCore.Domain.Entities;
using CampingCore.Domain.ReadModels;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : Repository<User, int>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default)
        => await Context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await Context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
        => await Context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await Context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> ExistsByEmailExceptUserIdAsync(
        string email,
        int excludeUserId,
        CancellationToken cancellationToken = default)
        => await Context.Users.AnyAsync(
            u => u.Email == email && u.Id != excludeUserId,
            cancellationToken);

    public async Task<IReadOnlyList<UserSystemListItem>> GetAllWithStatisticsAsync(
        CancellationToken cancellationToken = default)
        => await Context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new UserSystemListItem(
                u.Id,
                u.Name,
                u.Email,
                u.Role!.Name,
                u.CreatedAt,
                u.CreatedCampSites.Count,
                u.Trips.Count,
                u.Favorites.Count))
            .ToListAsync(cancellationToken);
}
