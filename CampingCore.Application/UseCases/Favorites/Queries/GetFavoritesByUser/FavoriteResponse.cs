using CampingCore.Application.UseCases.Shared;

namespace CampingCore.Application.Favorites.Queries.GetFavoritesByUser;

public record FavoriteResponse(
    int Id,
    int UserId,
    int CampSiteId,
    CampSiteSummary? CampSiteSummary);
