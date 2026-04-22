using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;

namespace CampingCore.Application.CampSites.Queries.GetAllCampSites;

public record GetAllCampSitesQuery : IQuery<IReadOnlyList<CampSiteResponse>>;
