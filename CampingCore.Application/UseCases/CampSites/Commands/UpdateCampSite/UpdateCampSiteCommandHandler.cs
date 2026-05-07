using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Application.UseCases.CampSites.Commands.UpdateCampSite;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Commands.UpdateCampSite;

internal sealed class UpdateCampSiteCommandHandler : ICommandHandler<UpdateCampSiteCommand>
{
    private static readonly Error Forbidden =
        new("CampSite.Forbidden", "No tienes permiso para modificar este sitio de camping.");

    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ICurrentUser        _currentUser;

    public UpdateCampSiteCommandHandler(
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _campSiteRepository = campSiteRepository;
        _unitOfWork         = unitOfWork;
        _currentUser        = currentUser;
    }

    public async Task<Result> Handle(UpdateCampSiteCommand request, CancellationToken cancellationToken)
    {
        var campSite = await _campSiteRepository.GetByIdAsync(request.Id, cancellationToken);

        if (campSite is null)
            return Result.Failure(Error.NotFound(nameof(CampSite), request.Id));

        if (!_currentUser.IsSuperUser && campSite.CreatedByUserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var updateResult = campSite.Update(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.HasWater,
            request.HasElectricity,
            request.IdProvincia,
            request.IdCanton,
            request.IdDistrito,
            request.DireccionExacta);

        if (updateResult.IsFailure)
            return updateResult;

        var newBase64 = request.NewImages.Select(i => i.Base64).ToList();
        var replace = campSite.ReplaceImages(request.ImageIdsToKeep, newBase64);
        if (replace.IsFailure)
            return replace;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
