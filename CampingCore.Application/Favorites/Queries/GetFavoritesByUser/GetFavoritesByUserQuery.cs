using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Favorites.Queries.GetFavoritesByUser;

public record GetFavoritesByUserQuery(int UserId) : IQuery<IReadOnlyList<FavoriteResponse>>;
