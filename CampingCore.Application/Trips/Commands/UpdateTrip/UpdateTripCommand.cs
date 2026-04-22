using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Commands.UpdateTrip;

public record UpdateTripCommand(
    int      Id,
    string   Name,
    DateOnly StartDate,
    DateOnly EndDate,
    int      RequestingUserId) : ICommand;
