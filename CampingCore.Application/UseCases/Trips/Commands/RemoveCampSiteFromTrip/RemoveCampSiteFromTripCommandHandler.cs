using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Commands.RemoveCampSiteFromTrip;

internal sealed class RemoveCampSiteFromTripCommandHandler : ICommandHandler<RemoveCampSiteFromTripCommand>
{
    private static readonly Error Forbidden =
        new("Trip.Forbidden", "No tienes permiso para modificar este viaje.");

    private static readonly Error NotInTrip =
        new("Trip.CampSiteNotFound", "El sitio de camping no forma parte de este viaje.");

    private readonly ITripRepository         _tripRepository;
    private readonly ITripCampSiteRepository _tripCampSiteRepository;
    private readonly IUnitOfWork             _unitOfWork;

    public RemoveCampSiteFromTripCommandHandler(
        ITripRepository tripRepository,
        ITripCampSiteRepository tripCampSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository         = tripRepository;
        _tripCampSiteRepository = tripCampSiteRepository;
        _unitOfWork             = unitOfWork;
    }

    public async Task<Result> Handle(RemoveCampSiteFromTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
            return Result.Failure(Error.NotFound(nameof(Trip), request.TripId));

        if (trip.UserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var tripCampSite = await _tripCampSiteRepository.GetByTripAndCampSiteAsync(
            request.TripId, request.CampSiteId, cancellationToken);

        if (tripCampSite is null)
            return Result.Failure(NotInTrip);

        _tripCampSiteRepository.Remove(tripCampSite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
