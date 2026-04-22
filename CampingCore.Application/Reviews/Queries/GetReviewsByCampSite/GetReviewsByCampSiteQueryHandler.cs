using AutoMapper;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

internal sealed class GetReviewsByCampSiteQueryHandler : IQueryHandler<GetReviewsByCampSiteQuery, IReadOnlyList<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByCampSiteQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ReviewResponse>>> Handle(GetReviewsByCampSiteQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByCampSiteIdAsync(request.CampSiteId, cancellationToken);

        return Result.Success<IReadOnlyList<ReviewResponse>>(_mapper.Map<IReadOnlyList<ReviewResponse>>(reviews));
    }
}
