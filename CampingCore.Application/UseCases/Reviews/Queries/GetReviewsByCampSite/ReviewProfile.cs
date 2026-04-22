using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

internal sealed class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewResponse>();
    }
}
