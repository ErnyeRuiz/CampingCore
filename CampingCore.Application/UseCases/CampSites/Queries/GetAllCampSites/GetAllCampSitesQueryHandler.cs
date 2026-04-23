using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Queries.GetAllCampSites;

internal sealed class GetAllCampSitesQueryHandler : IQueryHandler<GetAllCampSitesQuery, IReadOnlyList<CampSiteResponse>>
{
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IMapper _mapper;

    public GetAllCampSitesQueryHandler(
        ICampSiteRepository campSiteRepository, 
        IMapper mapper)
    {
        _campSiteRepository = campSiteRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<CampSiteResponse>>> Handle(GetAllCampSitesQuery request, CancellationToken cancellationToken)
    {
        var campSites = await _campSiteRepository.GetAllAsync(cancellationToken);

        return Result.Success(_mapper.Map<IReadOnlyList<CampSiteResponse>>(campSites));
    }
}
