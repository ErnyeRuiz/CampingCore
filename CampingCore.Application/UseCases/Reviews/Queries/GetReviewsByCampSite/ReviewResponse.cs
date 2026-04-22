namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

public record ReviewResponse(int Id, int UserId, int CampSiteId, byte Rating, string? Comment, DateTime CreatedAt);
