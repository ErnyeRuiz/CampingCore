namespace CampingCore.Application.CampSites;

/// <summary>
/// Imagen recibida en un comando.
/// </summary>
public sealed record ImageFileDto(byte[] Bytes, string? ContentType, string? FileName);
