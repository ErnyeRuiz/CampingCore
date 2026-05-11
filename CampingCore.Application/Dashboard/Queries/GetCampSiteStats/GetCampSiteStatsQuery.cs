using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Dashboard.Queries.GetCampSiteStats;

public record GetCampSiteStatsQuery(
    int? IdProvincia,
    int? IdCanton,
    int? IdDistrito) : IQuery<CampSiteStatsResponse>;
