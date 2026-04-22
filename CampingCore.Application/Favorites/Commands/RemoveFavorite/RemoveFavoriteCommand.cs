using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Favorites.Commands.RemoveFavorite;

public record RemoveFavoriteCommand(int UserId, int CampSiteId) : ICommand;
