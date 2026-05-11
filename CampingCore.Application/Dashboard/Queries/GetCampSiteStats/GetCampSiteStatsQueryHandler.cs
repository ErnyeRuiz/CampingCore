using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Dashboard.Queries.GetCampSiteStats;

internal sealed class GetCampSiteStatsQueryHandler
    : IQueryHandler<GetCampSiteStatsQuery, CampSiteStatsResponse>
{
    private readonly ICampSiteRepository _campSiteRepository;

    public GetCampSiteStatsQueryHandler(ICampSiteRepository campSiteRepository) =>
        _campSiteRepository = campSiteRepository;

    public async Task<Result<CampSiteStatsResponse>> Handle(
        GetCampSiteStatsQuery request,
        CancellationToken cancellationToken)
    {
        var (totalCount, averageRating) = await _campSiteRepository.GetStatsAsync(
            request.IdProvincia,
            request.IdCanton,
            request.IdDistrito,
            cancellationToken);

        return Result.Success(new CampSiteStatsResponse(totalCount, averageRating));
    }
}
