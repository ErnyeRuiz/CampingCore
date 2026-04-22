using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.CampSites.Commands.CreateCampSite;

public record CreateCampSiteCommand(
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool HasWater,
    bool HasElectricity,
    int CreatedByUserId) : ICommand<int>;
