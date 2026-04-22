using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Commands.UpdateTrip;

internal sealed class UpdateTripCommandHandler : ICommandHandler<UpdateTripCommand>
{
    private static readonly Error Forbidden =
        new("Trip.Forbidden", "No tienes permiso para modificar este viaje.");

    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public UpdateTripCommandHandler(ITripRepository tripRepository, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.Id, cancellationToken);

        if (trip is null)
            return Result.Failure(Error.NotFound(nameof(Trip), request.Id));

        if (trip.UserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var result = trip.Update(request.Name, request.StartDate, request.EndDate);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
