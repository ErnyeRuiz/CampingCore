using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class TripCampSite
{
    public static class Errors
    {
        public static readonly Error InvalidTripId     = Error.Validation("TripCampSite.InvalidTripId",     "El identificador del viaje debe ser mayor a 0.");
        public static readonly Error InvalidCampSiteId = Error.Validation("TripCampSite.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
    }

    public int Id { get; private set; }
    public int TripId { get; private set; }
    public int CampSiteId { get; private set; }

    public Trip? Trip { get; private set; }
    public CampSite? CampSite { get; private set; }

    protected TripCampSite() { }

    private TripCampSite(int tripId, int campSiteId)
    {
        TripId = tripId;
        CampSiteId = campSiteId;
    }

    public static Result<TripCampSite> Create(int tripId, int campSiteId)
    {
        if (tripId <= 0)     return Result.Failure<TripCampSite>(Errors.InvalidTripId);
        if (campSiteId <= 0) return Result.Failure<TripCampSite>(Errors.InvalidCampSiteId);

        return new TripCampSite(tripId, campSiteId);
    }
}
