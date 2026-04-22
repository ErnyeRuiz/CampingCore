using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(int UserId, int CampSiteId, byte Rating, string? Comment) : ICommand<int>;
