using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Favorites.Commands.AddFavorite;

public record AddFavoriteCommand(int UserId, int CampSiteId) : ICommand<int>;
