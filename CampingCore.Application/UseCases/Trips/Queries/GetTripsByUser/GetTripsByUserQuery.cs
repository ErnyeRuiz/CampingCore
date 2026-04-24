using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.UseCases.Trips.Queries.GetTripById;

namespace CampingCore.Application.Trips.Queries.GetTripsByUser;

public record GetTripsByUserQuery(int UserId) : IQuery<IReadOnlyList<TripResponse>>;
