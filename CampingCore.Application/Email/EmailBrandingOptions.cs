namespace CampingCore.Application.Email;

public sealed class EmailBrandingOptions
{
    public const string SectionName = "EmailBranding";

    /// <summary>Base URL pública del API (HTTPS), sin barra final. Ej: https://localhost:7061</summary>
    public string PublicBaseUrl { get; init; } = string.Empty;

    /// <summary>Color de acento (hex) para separadores y enlaces; debe coincidir con el azul del logo.</summary>
    public string AccentColor { get; init; } = "#2563eb";

    /// <summary>Ruta bajo wwwroot, ej. images/dark_and_title.png</summary>
    public string LogoRelativePath { get; init; } = "images/dark_and_title.png";

    public string? SupportEmail { get; init; }

    public string? SupportFaqUrl { get; init; }

    /// <summary>Texto legal breve para el pie (texto plano).</summary>
    public string? LegalNotice { get; init; }
}
