using CampingCore.Application.Users.Commands.UpdateUser;
using CampingCore.Application.Users.Queries.GetUserById;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Perfil del usuario autenticado (<c>GET/PUT /api/users/me</c>).
/// </summary>
[Route("api/users")]
[Authorize]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender) { }

    /// <summary>
    /// Devuelve el perfil del usuario cuyo id coincide con el claim <c>sub</c> del JWT.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="UserResponse"/> (<c>id</c>, <c>name</c>, <c>email</c>, <c>createdAt</c>, <c>roleName</c>); <c>404</c> si no hay coincidencia con el token.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUserByIdQuery(GetCurrentUserId()), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Actualiza el nombre del usuario autenticado.
    /// </summary>
    /// <param name="request">Nuevo <c>name</c> mostrable.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>400/404</c> si validación o usuario no existe.</returns>
    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateUserCommand(request.Name, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Perfil actualizado exitosamente.");
    }
}

/// <param name="Name">Nombre completo o visible (máx. 100 caracteres).</param>
public record UpdateUserRequest(string Name);
