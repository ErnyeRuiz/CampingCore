using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Users.Queries.GetUserById;

internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));
    }
}
