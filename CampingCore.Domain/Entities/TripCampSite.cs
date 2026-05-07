using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class TripCampSite : Entity<int>
{
    public static class Errors
    {
        public static readonly Error InvalidTripId     = Error.Validation("TripCampSite.InvalidTripId",     "El identificador del viaje debe ser mayor a 0.");
        public static readonly Error InvalidCampSiteId = Error.Validation("TripCampSite.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
    }

    public int TripId { get; private set; }
    public int CampSiteId { get; private set; }

    public Trip? Trip { get; private set; }
    public CampSite? CampSite { get; private set; }

    protected TripCampSite() : base(0) { }

    private TripCampSite(int tripId, int campSiteId) : base(0)
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
