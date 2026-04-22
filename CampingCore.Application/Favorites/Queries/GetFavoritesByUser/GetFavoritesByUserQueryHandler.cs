using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Favorites.Queries.GetFavoritesByUser;

internal sealed class GetFavoritesByUserQueryHandler : IQueryHandler<GetFavoritesByUserQuery, IReadOnlyList<FavoriteResponse>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IMapper _mapper;

    public GetFavoritesByUserQueryHandler(IFavoriteRepository favoriteRepository, IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<FavoriteResponse>>> Handle(GetFavoritesByUserQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return Result.Success<IReadOnlyList<FavoriteResponse>>(_mapper.Map<IReadOnlyList<FavoriteResponse>>(favorites));
    }
}
