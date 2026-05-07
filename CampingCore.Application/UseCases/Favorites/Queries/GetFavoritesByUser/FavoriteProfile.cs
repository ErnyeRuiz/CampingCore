using AutoMapper;
using CampingCore.Application.Favorites.Queries.GetFavoritesByUser;
using CampingCore.Application.UseCases.Shared;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.UseCases.Favorites.Queries.GetFavoritesByUser;

internal sealed class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        CreateMap<Favorite, FavoriteResponse>()
            .ConstructUsing((src, ctx) => new FavoriteResponse(
                src.Id,
                src.UserId,
                src.CampSiteId,
                src.CampSite != null ? ctx.Mapper.Map<CampSiteSummary>(src.CampSite) : null));
    }
}
