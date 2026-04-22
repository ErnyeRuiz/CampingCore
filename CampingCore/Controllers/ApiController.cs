using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Base para controladores que usan el claim <c>sub</c> del JWT como identificador de usuario.
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender) => Sender = sender;

    /// <summary>
    /// Devuelve el <c>UserId</c> a partir del token (claim <c>sub</c> o <c>nameIdentifier</c>).
    /// </summary>
    /// <returns>Identificador entero del usuario autenticado.</returns>
    protected int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.Parse(claim!);
    }
}
