using CampingCore.Application.Users.Commands.Login;
using CampingCore.Application.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Registro e inicio de sesión. Devuelve JWT en login para usar en <c>Authorization: Bearer {token}</c>.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    /// <summary>
    /// Crea un usuario y persiste su contraseña con hash. No inicia sesión: usa <c>POST /api/auth/login</c> para obtener el token.
    /// </summary>
    /// <param name="command">Cuerpo con <c>name</c>, <c>email</c> y <c>password</c> en texto claro (se hashea en servidor).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> del usuario; <c>400</c> si el email ya existe o validación falla.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(Register), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Autentica por email y contraseña. Respuesta incluye <c>token</c> JWT.
    /// </summary>
    /// <param name="command">Cuerpo con <c>email</c> y <c>password</c>.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="LoginResponse"/>; <c>401</c> si credenciales incorrectas.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return Unauthorized(result.Error);

        return Ok(result.Value);
    }
}
