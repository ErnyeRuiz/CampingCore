using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CampingCore.Common;
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
    protected int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.Parse(claim!);
    }

    /// <summary>Respuesta 200 con datos envueltos en <see cref="ApiResponse{T}"/>.</summary>
    protected IActionResult OkResponse<T>(T data, string message = "Operación exitosa.")
        => Ok(ApiResponse.Success(data, 200, message));

    /// <summary>Respuesta 201 con datos envueltos en <see cref="ApiResponse{T}"/> y cabecera <c>Location</c>.</summary>
    protected IActionResult CreatedResponse<T>(string action, object routeValues, T data)
        => CreatedAtAction(action, routeValues, ApiResponse.Success(data, 201, "Recurso creado exitosamente."));

    /// <summary>Respuesta 200 sin payload de datos (operaciones sin retorno).</summary>
    protected IActionResult SuccessResponse(string message = "Operación completada.")
        => Ok(ApiResponse.Success<object?>(null, 200, message));

    /// <summary>
    /// Mapea un <see cref="Domain.Common.Error"/> al código HTTP correspondiente y lo envuelve
    /// en <see cref="ApiResponse{T}"/> con <c>data</c> nulo.
    /// </summary>
    protected IActionResult MapErrorResponse(Domain.Common.Error error) => error.Code switch
    {
        var c when c.EndsWith(".NotFound")  => NotFound(ApiResponse.Fail(404, error.Description)),
        var c when c.EndsWith(".Forbidden") => StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(403, error.Description)),
        _                                   => BadRequest(ApiResponse.Fail(400, error.Description))
    };
}
