using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Commands.DeleteTrip;

internal sealed class DeleteTripCommandHandler : ICommandHandler<DeleteTripCommand>
{
    private static readonly Error Forbidden =
        new("Trip.Forbidden", "No tienes permiso para eliminar este viaje.");

    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public DeleteTripCommandHandler(ITripRepository tripRepository, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.Id, cancellationToken);

        if (trip is null)
            return Result.Failure(Error.NotFound(nameof(Trip), request.Id));

        if (trip.UserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        _tripRepository.Remove(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
