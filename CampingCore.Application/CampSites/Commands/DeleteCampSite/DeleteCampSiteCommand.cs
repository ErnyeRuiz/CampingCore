using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.CampSites.Commands.DeleteCampSite;

public record DeleteCampSiteCommand(int Id, int RequestingUserId) : ICommand;
