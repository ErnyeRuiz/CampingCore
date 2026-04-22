using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Queries.GetCampSiteById;

internal sealed class GetCampSiteByIdQueryHandler : IQueryHandler<GetCampSiteByIdQuery, CampSiteResponse>
{
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IMapper _mapper;

    public GetCampSiteByIdQueryHandler(ICampSiteRepository campSiteRepository, IMapper mapper)
    {
        _campSiteRepository = campSiteRepository;
        _mapper = mapper;
    }

    public async Task<Result<CampSiteResponse>> Handle(GetCampSiteByIdQuery request, CancellationToken cancellationToken)
    {
        var campSite = await _campSiteRepository.GetByIdAsync(request.CampSiteId, cancellationToken);

        if (campSite is null)
            return Result.Failure<CampSiteResponse>(Error.NotFound(nameof(CampSite), request.CampSiteId));

        return _mapper.Map<CampSiteResponse>(campSite);
    }
}
