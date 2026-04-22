using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class Trip
{
    public static class Errors
    {
        public static readonly Error InvalidUserId     = Error.Validation("Trip.InvalidUserId",     "El identificador del usuario debe ser mayor a 0.");
        public static readonly Error NameRequired      = Error.Validation("Trip.NameRequired",      "El nombre del viaje es obligatorio.");
        public static readonly Error NameTooLong       = Error.Validation("Trip.NameTooLong",       "El nombre del viaje no puede superar 100 caracteres.");
        public static readonly Error InvalidDateRange  = Error.Validation("Trip.InvalidDateRange",  "La fecha de fin debe ser igual o posterior a la fecha de inicio.");
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public User? User { get; private set; }
    public IReadOnlyList<TripCampSite> CampSites { get; private set; } = [];

    protected Trip() { }

    private Trip(int userId, string name, DateOnly startDate, DateOnly endDate)
    {
        UserId = userId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<Trip> Create(int userId, string name, DateOnly startDate, DateOnly endDate)
    {
        if (userId <= 0)                      return Result.Failure<Trip>(Errors.InvalidUserId);
        if (string.IsNullOrWhiteSpace(name))  return Result.Failure<Trip>(Errors.NameRequired);
        if (name.Length > 100)                return Result.Failure<Trip>(Errors.NameTooLong);
        if (endDate < startDate)              return Result.Failure<Trip>(Errors.InvalidDateRange);

        return new Trip(userId, name, startDate, endDate);
    }

    public Result Update(string name, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure(Errors.NameTooLong);
        if (endDate < startDate)             return Result.Failure(Errors.InvalidDateRange);

        Name      = name;
        StartDate = startDate;
        EndDate   = endDate;

        return Result.Success();
    }
}
