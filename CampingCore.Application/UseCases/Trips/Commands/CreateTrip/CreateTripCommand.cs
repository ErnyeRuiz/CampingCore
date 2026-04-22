using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Commands.CreateTrip;

public record CreateTripCommand(int UserId, string Name, DateOnly StartDate, DateOnly EndDate) : ICommand<int>;
