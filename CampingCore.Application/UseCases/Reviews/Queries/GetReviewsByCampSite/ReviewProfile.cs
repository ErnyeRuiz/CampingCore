using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;

internal sealed class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewResponse>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? s.User.Name : null));
    }
}
