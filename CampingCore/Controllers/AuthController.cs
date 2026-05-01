using CampingCore.Application.Users.Commands.Login;
using CampingCore.Application.Users.Commands.RegisterUser;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Registro e inicio de sesión. Devuelve JWT en login para usar en <c>Authorization: Bearer {token}</c>.
/// </summary>
[Route("api/auth")]
public sealed class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender) { }

    /// <summary>
    /// Crea un usuario con contraseña hasheada y le asigna el rol <c>Customer</c> si está definido en el sistema.
    /// No devuelve JWT: usa <c>POST /api/auth/login</c> después del registro.
    /// </summary>
    /// <param name="command">Cuerpo con <c>name</c>, <c>email</c> y <c>password</c> en texto claro (se hashea en servidor).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> del usuario; <c>400</c> si el email ya existe, validación falla o falta el rol <c>Customer</c>.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return CreatedResponse(nameof(Register), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Autentica por email y contraseña. Cuerpo de éxito: <c>userId</c>, <c>name</c>, <c>email</c>, <c>roleName</c> y <c>token</c> JWT (con rol y claims <c>permission</c> si aplica).
    /// </summary>
    /// <param name="command">Cuerpo con <c>email</c> y <c>password</c>.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="LoginResponse"/>; <c>401</c> si credenciales incorrectas.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse.Fail(401, result.Error.Description));

        return OkResponse(result.Value);
    }
}
