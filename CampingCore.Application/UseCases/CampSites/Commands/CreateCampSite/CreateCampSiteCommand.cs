using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.CampSites;

namespace CampingCore.Application.UseCases.CampSites.Commands.CreateCampSite
{
    public record CreateCampSiteCommand(
        string Name,
        string? Description,
        decimal Latitude,
        decimal Longitude,
        decimal PricePerNight,
        bool HasWater,
        bool HasElectricity,
        int CreatedByUserId,
        int IdProvincia,
        int IdCanton,
        int IdDistrito,
        string? DireccionExacta,
        IReadOnlyList<ImageFileDto> NewImages) : ICommand<int>;
}
