using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

public record GetReviewsByCampSiteQuery(int CampSiteId) : IQuery<IReadOnlyList<ReviewResponse>>;
