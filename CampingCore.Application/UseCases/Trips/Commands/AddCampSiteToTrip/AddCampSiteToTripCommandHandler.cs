using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Commands.AddCampSiteToTrip;

internal sealed class AddCampSiteToTripCommandHandler : ICommandHandler<AddCampSiteToTripCommand, int>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly ITripCampSiteRepository _tripCampSiteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCampSiteToTripCommandHandler(
        ITripRepository tripRepository,
        ICampSiteRepository campSiteRepository,
        ITripCampSiteRepository tripCampSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _campSiteRepository = campSiteRepository;
        _tripCampSiteRepository = tripCampSiteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AddCampSiteToTripCommand request, CancellationToken cancellationToken)
    {
        if (await _tripRepository.GetByIdAsync(request.TripId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(Trip), request.TripId));

        if (await _campSiteRepository.GetByIdAsync(request.CampSiteId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(CampSite), request.CampSiteId));

        if (await _tripCampSiteRepository.ExistsAsync(request.TripId, request.CampSiteId, cancellationToken))
            return Result.Failure<int>(new Error("TripCampSite.AlreadyExists", "El sitio de camping ya está agregado a este viaje."));

        var result = TripCampSite.Create(request.TripId, request.CampSiteId);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _tripCampSiteRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
