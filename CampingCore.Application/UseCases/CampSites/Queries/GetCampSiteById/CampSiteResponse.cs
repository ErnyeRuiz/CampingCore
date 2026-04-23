namespace CampingCore.Application.CampSites.Queries.GetCampSiteById;

public record CampSiteResponse(
    int Id,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool HasWater,
    bool HasElectricity,
    int CreatedByUserId,
    DateTime CreatedAt,
    decimal Rating,
    int IdProvincia,
    int IdCanton,
    int IdDistrito,
    string? DireccionExacta);
