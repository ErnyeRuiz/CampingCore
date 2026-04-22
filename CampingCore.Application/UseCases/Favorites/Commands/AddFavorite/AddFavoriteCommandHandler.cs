using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Favorites.Commands.AddFavorite;

internal sealed class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand, int>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFavoriteCommandHandler(
        IFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _campSiteRepository = campSiteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(User), request.UserId));

        if (await _campSiteRepository.GetByIdAsync(request.CampSiteId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(CampSite), request.CampSiteId));

        var existing = await _favoriteRepository.GetByUserAndCampSiteAsync(request.UserId, request.CampSiteId, cancellationToken);

        if (existing is not null)
            return Result.Failure<int>(new Error("Favorite.AlreadyExists", "Este sitio de camping ya está en favoritos."));

        var result = Favorite.Create(request.UserId, request.CampSiteId);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _favoriteRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
