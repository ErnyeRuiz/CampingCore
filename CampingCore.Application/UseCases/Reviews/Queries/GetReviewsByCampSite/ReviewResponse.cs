namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

public record ReviewResponse(int Id, int UserId, string? UserName, int CampSiteId, byte Rating, string? Comment, DateTime CreatedAt);
