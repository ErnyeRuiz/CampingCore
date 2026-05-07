using System.Text.Json.Serialization;

namespace CampingCore.Domain.ReadModels;

/// <summary>
/// Fila de listado administrativo de usuarios (solo lectura, sin contraseña).
/// </summary>
public sealed record UserSystemListItem(
    int Id,
    string Name,
    string Email,
    string RoleName,
    DateTime CreatedAt,
    [property: JsonPropertyName("campsites")] int CampSitesCount,
    [property: JsonPropertyName("tripsAmount")] int TripsCount,
    [property: JsonPropertyName("favorites")] int FavoritesCount);
