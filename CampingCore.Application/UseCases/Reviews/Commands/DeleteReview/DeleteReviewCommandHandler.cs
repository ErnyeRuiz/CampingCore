using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Reviews.Commands.DeleteReview;

internal sealed class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
{
    private static readonly Error Forbidden =
        new("Review.Forbidden", "No tienes permiso para eliminar esta reseña.");

    private readonly IReviewRepository  _reviewRepository;
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork        _unitOfWork;

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository   = reviewRepository;
        _campSiteRepository = campSiteRepository;
        _unitOfWork         = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review is null)
            return Result.Failure(Error.NotFound(nameof(Review), request.Id));

        if (review.UserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var campSiteId = review.CampSiteId;

        _reviewRepository.Remove(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _campSiteRepository.RecalculateCampSiteRatingAsync(campSiteId, cancellationToken);

        return Result.Success();
    }
}
