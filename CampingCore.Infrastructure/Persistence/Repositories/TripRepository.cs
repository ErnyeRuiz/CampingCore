using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class TripRepository : Repository<Trip, int>, ITripRepository
{
    public TripRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await Context.Trips
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
}
