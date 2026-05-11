using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Reviews.Commands.CreateReview;

internal sealed class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, int>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _campSiteRepository = campSiteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(User), request.UserId));

        if (await _campSiteRepository.GetByIdAsync(request.CampSiteId, cancellationToken) is null)
            return Result.Failure<int>(Error.NotFound(nameof(CampSite), request.CampSiteId));

        var result = Review.Create(request.UserId, request.CampSiteId, request.Rating, request.Comment);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _reviewRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
