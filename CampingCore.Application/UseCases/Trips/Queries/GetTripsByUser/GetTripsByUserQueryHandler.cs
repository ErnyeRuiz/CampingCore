using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Trips.Queries.GetTripById;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Trips.Queries.GetTripsByUser;

internal sealed class GetTripsByUserQueryHandler : IQueryHandler<GetTripsByUserQuery, IReadOnlyList<TripResponse>>
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;

    public GetTripsByUserQueryHandler(ITripRepository tripRepository, IMapper mapper)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<TripResponse>>> Handle(GetTripsByUserQuery request, CancellationToken cancellationToken)
    {
        var trips = await _tripRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return Result.Success<IReadOnlyList<TripResponse>>(_mapper.Map<IReadOnlyList<TripResponse>>(trips));
    }
}
