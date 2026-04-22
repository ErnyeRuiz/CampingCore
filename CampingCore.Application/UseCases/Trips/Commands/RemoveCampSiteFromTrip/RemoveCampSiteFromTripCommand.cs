using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Commands.RemoveCampSiteFromTrip;

public record RemoveCampSiteFromTripCommand(int TripId, int CampSiteId, int RequestingUserId) : ICommand;
