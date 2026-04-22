using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Favorites.Commands.RemoveFavorite;

internal sealed class RemoveFavoriteCommandHandler : ICommandHandler<RemoveFavoriteCommand>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFavoriteCommandHandler(IFavoriteRepository favoriteRepository, IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await _favoriteRepository.GetByUserAndCampSiteAsync(request.UserId, request.CampSiteId, cancellationToken);

        if (favorite is null)
            return Result.Failure(new Error("Favorite.NotFound", "El favorito no existe."));

        _favoriteRepository.Remove(favorite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
