namespace CampingCore.Application.Options;

/// <summary>
/// Opciones de sesión enlazadas a la sección <c>JwtSettings</c> (solo las necesarias en Application).
/// </summary>
public sealed class SessionAuthSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Días sin usar refresh antes de exigir login de nuevo (ventana deslizante por último uso).
    /// </summary>
    public int RefreshIdleTimeoutDays { get; set; } = 30;
}
