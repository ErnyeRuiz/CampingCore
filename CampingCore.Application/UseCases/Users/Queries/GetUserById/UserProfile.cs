using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Users.Queries.GetUserById;

internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
