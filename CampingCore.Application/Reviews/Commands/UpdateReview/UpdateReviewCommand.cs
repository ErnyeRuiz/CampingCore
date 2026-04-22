using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(int Id, byte Rating, string? Comment, int RequestingUserId) : ICommand;
