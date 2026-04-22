namespace CampingCore.Application.Trips.Queries.GetTripById;

public record TripResponse(int Id, int UserId, string Name, DateOnly StartDate, DateOnly EndDate);
