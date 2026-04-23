using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.CampSites.Commands.UpdateCampSite;

public record UpdateCampSiteCommand(
    int     Id,
    string  Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool    HasWater,
    bool    HasElectricity,
    int     RequestingUserId,
    int     IdProvincia,
    int     IdCanton,
    int     IdDistrito,
    string? DireccionExacta) : ICommand;
