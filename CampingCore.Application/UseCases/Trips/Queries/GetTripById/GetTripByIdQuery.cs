using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.UseCases.Trips.Queries.GetTripById;

namespace CampingCore.Application.Trips.Queries.GetTripById;

public record GetTripByIdQuery(int TripId) : IQuery<TripResponse>;
