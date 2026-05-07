using CampingCore.Domain.Entities;
using CampingCore.Domain.ReadModels;

namespace CampingCore.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailExceptUserIdAsync(string email, int excludeUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserSystemListItem>> GetAllWithStatisticsAsync(CancellationToken cancellationToken = default);
    void Add(User user);
}
