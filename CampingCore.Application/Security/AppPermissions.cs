using System.Globalization;

namespace CampingCore.Application.Security;

/// <summary>
/// Nombres de permisos (tabla <c>Permissions</c> / claims JWT tipo <c>permission</c>).
/// Políticas ASP.NET: prefijo <see cref="PolicyPrefix"/> + nombre.
/// </summary>
public static class AppPermissions
{
    public const string CreateCampSite = "campsite.create";
    public const string UpdateCampSite = "campsite.update";
    public const string DeleteCampSite = "campsite.delete";

    public const string PolicyPrefix = "Permission:";

    public const string CreateCampSitePolicy = PolicyPrefix + CreateCampSite;
    public const string UpdateCampSitePolicy = PolicyPrefix + UpdateCampSite;
    public const string DeleteCampSitePolicy = PolicyPrefix + DeleteCampSite;

    public static string Policy(string permissionName) => PolicyPrefix + permissionName;
}
