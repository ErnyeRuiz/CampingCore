using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class Review : Entity<int>
{
    public static class Errors
    {
        public static readonly Error InvalidUserId     = Error.Validation("Review.InvalidUserId",     "El identificador del usuario debe ser mayor a 0.");
        public static readonly Error InvalidCampSiteId = Error.Validation("Review.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
        public static readonly Error RatingOutOfRange  = Error.Validation("Review.RatingOutOfRange",  "La calificación debe estar entre 1 y 5.");
    }

    public int UserId { get; private set; }
    public int CampSiteId { get; private set; }
    public byte Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User? User { get; private set; }
    public CampSite? CampSite { get; private set; }

    protected Review() : base(0) { }

    private Review(int userId, int campSiteId, byte rating, string? comment) : base(0)
    {
        UserId = userId;
        CampSiteId = campSiteId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Review> Create(int userId, int campSiteId, byte rating, string? comment)
    {
        if (userId <= 0)              return Result.Failure<Review>(Errors.InvalidUserId);
        if (campSiteId <= 0)          return Result.Failure<Review>(Errors.InvalidCampSiteId);
        if (rating < 1 || rating > 5) return Result.Failure<Review>(Errors.RatingOutOfRange);

        return new Review(userId, campSiteId, rating, comment);
    }

    public Result Update(byte rating, string? comment)
    {
        if (rating < 1 || rating > 5) return Result.Failure(Errors.RatingOutOfRange);

        Rating  = rating;
        Comment = comment;

        return Result.Success();
    }
}
