using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Commands.DeleteTrip;

public record DeleteTripCommand(int Id, int RequestingUserId) : ICommand;
