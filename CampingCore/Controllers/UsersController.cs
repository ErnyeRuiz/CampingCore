using CampingCore.Application.Users.Commands.UpdateUser;
using CampingCore.Application.Users.Queries.GetUserById;
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
    /// <returns><c>200</c> con <see cref="UserResponse"/>; <c>404</c> si el usuario no existe.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUserByIdQuery(GetCurrentUserId()), cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Actualiza el nombre del usuario autenticado.
    /// </summary>
    /// <param name="request">Nuevo <c>name</c> mostrable.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> sin cuerpo; <c>404</c> si el usuario no existe.</returns>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateUserCommand(request.Name, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok();
    }
}

/// <param name="Name">Nombre completo o visible (máx. 100 caracteres).</param>
public record UpdateUserRequest(string Name);
