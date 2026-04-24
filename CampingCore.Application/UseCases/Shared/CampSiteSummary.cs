namespace CampingCore.Application.UseCases.Shared;

/// <summary>
/// Resumen de un sitio de camping vinculado a un viaje (sin galería completa; <see cref="Image"/> es la primera imagen, p. ej. para miniatura).
/// </summary>
public record CampSiteSummary(
    int Id,
    string Name,
    string? Description,
    decimal PricePerNight,
    int IdProvincia,
    int IdCanton,
    int IdDistrito,
    string? Image);
