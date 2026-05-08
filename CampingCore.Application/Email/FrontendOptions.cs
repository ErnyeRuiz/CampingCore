namespace CampingCore.Application.Email;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    /// <summary>Base URL del front (SPA), sin barra final. Ej: https://app.ejemplo.com</summary>
    public string BaseUrl { get; init; } = string.Empty;
}
