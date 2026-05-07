namespace CampingCore.Application.CampSites;

/// <summary>
/// Imagen recibida en un comando (p. ej. mapeada desde multipart en el API). Sin tipos de ASP.NET Core.
/// </summary>
public sealed record ImageFileDto(string Base64, string? ContentType, string? FileName);
