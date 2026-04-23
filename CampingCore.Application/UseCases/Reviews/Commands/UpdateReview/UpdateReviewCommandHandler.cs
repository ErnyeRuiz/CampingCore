using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Reviews.Commands.UpdateReview;

internal sealed class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand>
{
    private static readonly Error Forbidden =
        new("Review.Forbidden", "No tienes permiso para modificar esta reseña.");

    private readonly IReviewRepository  _reviewRepository;
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork        _unitOfWork;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository    = reviewRepository;
        _campSiteRepository  = campSiteRepository;
        _unitOfWork          = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review is null)
            return Result.Failure(Error.NotFound(nameof(Review), request.Id));

        if (review.UserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var result = review.Update(request.Rating, request.Comment);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _campSiteRepository.RecalculateCampSiteRatingAsync(review.CampSiteId, cancellationToken);

        return Result.Success();
    }
}
