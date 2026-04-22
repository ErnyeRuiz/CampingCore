using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepository : Repository<Review, int>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Review>> GetByCampSiteIdAsync(int campSiteId, CancellationToken cancellationToken = default)
        => await Context.Reviews
            .Where(r => r.CampSiteId == campSiteId)
            .ToListAsync(cancellationToken);
}
