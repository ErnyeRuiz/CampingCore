using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Queries.GetManagedCampSites;

internal sealed class GetManagedCampSitesQueryHandler : IQueryHandler<GetManagedCampSitesQuery, IReadOnlyList<CampSiteResponse>>
{
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetManagedCampSitesQueryHandler(
        ICampSiteRepository campSiteRepository,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _campSiteRepository = campSiteRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<CampSiteResponse>>> Handle(GetManagedCampSitesQuery request, CancellationToken cancellationToken)
    {
        var campSites = _currentUser.IsSuperUser
            ? await _campSiteRepository.GetAllAsync(cancellationToken)
            : await _campSiteRepository.GetByCreatedByUserIdAsync(_currentUser.UserId, cancellationToken);

        return Result.Success(_mapper.Map<IReadOnlyList<CampSiteResponse>>(campSites));
    }
}
