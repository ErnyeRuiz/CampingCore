using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Trips.Commands.AddCampSiteToTrip;

public record AddCampSiteToTripCommand(int TripId, int CampSiteId) : ICommand<int>;
