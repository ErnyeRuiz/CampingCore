using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;

namespace CampingCore.Application.CampSites.Queries.GetManagedCampSites;

public record GetManagedCampSitesQuery : IQuery<IReadOnlyList<CampSiteResponse>>;
