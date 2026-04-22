using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Queries.GetTripById;

public record GetTripByIdQuery(int TripId) : IQuery<TripResponse>;
