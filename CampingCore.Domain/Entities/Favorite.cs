using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class Favorite : Entity<int>
{
    public static class Errors
    {
        public static readonly Error InvalidUserId     = Error.Validation("Favorite.InvalidUserId",     "El identificador del usuario debe ser mayor a 0.");
        public static readonly Error InvalidCampSiteId = Error.Validation("Favorite.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
    }

    public int UserId { get; private set; }
    public int CampSiteId { get; private set; }

    public User? User { get; private set; }
    public CampSite? CampSite { get; private set; }

    protected Favorite() : base(0) { }

    private Favorite(int userId, int campSiteId) : base(0)
    {
        UserId = userId;
        CampSiteId = campSiteId;
    }

    public static Result<Favorite> Create(int userId, int campSiteId)
    {
        if (userId <= 0)     return Result.Failure<Favorite>(Errors.InvalidUserId);
        if (campSiteId <= 0) return Result.Failure<Favorite>(Errors.InvalidCampSiteId);

        return new Favorite(userId, campSiteId);
    }
}
