using CampingCore.Application.Abstractions.Messaging;
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

    public UpdateCampSiteCommandHandler(ICampSiteRepository campSiteRepository, IUnitOfWork unitOfWork)
    {
        _campSiteRepository = campSiteRepository;
        _unitOfWork         = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCampSiteCommand request, CancellationToken cancellationToken)
    {
        var campSite = await _campSiteRepository.GetByIdAsync(request.Id, cancellationToken);

        if (campSite is null)
            return Result.Failure(Error.NotFound(nameof(CampSite), request.Id));

        if (campSite.CreatedByUserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var result = campSite.Update(
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

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
