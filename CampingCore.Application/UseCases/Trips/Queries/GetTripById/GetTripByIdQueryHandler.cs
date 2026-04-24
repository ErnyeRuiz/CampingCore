using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.UseCases.Trips.Queries.GetTripById;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Queries.GetTripById;

internal sealed class GetTripByIdQueryHandler : IQueryHandler<GetTripByIdQuery, TripResponse>
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;

    public GetTripByIdQueryHandler(ITripRepository tripRepository, IMapper mapper)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
    }

    public async Task<Result<TripResponse>> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
            return Result.Failure<TripResponse>(Error.NotFound(nameof(Trip), request.TripId));

        return _mapper.Map<TripResponse>(trip);
    }
}
