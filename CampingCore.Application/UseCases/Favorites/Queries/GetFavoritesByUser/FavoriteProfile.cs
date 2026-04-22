using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Favorites.Queries.GetFavoritesByUser;

internal sealed class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        CreateMap<Favorite, FavoriteResponse>();
    }
}
