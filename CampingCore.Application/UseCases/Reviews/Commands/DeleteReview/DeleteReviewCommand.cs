using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(int Id, int RequestingUserId) : ICommand;
