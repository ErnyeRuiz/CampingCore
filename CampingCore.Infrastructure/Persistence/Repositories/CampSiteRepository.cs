using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Infrastructure.Persistence.Repositories;

internal sealed class CampSiteRepository : Repository<CampSite, int>, ICampSiteRepository
{
    public CampSiteRepository(ApplicationDbContext context) : base(context) { }
}
