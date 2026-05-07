using CampingCore.Application.UseCases.Shared;

namespace CampingCore.Application.UseCases.Trips.Queries.GetTripById;

public record TripResponse(
    int Id,
    int UserId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<CampSiteSummary> CampSiteSummaries);
