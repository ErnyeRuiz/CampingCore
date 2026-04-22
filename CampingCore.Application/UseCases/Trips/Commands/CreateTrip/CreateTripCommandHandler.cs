using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Commands.CreateTrip;

internal sealed class CreateTripCommandHandler : ICommandHandler<CreateTripCommand, int>
{
    private readonly ITripRepository _tripRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTripCommandHandler(
        ITripRepository tripRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(User), request.UserId));

        var result = Trip.Create(request.UserId, request.Name, request.StartDate, request.EndDate);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _tripRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
