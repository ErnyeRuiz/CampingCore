namespace CampingCore.Application.Abstractions.Security;

/// <summary>
/// Usuario de la petición HTTP actual (claims del JWT).
/// </summary>
public interface ICurrentUser
{
    int UserId { get; }
    bool IsSuperUser { get; }
}
