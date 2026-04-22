using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.CampSites.Queries.GetCampSiteById;

public record GetCampSiteByIdQuery(int CampSiteId) : IQuery<CampSiteResponse>;
