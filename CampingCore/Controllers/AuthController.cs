using CampingCore.Application.UseCases.Auth.ForgotPassword;
using CampingCore.Application.UseCases.Auth.Logout;
using CampingCore.Application.UseCases.Auth.RefreshSession;
using CampingCore.Application.UseCases.Auth.ResetPassword;
using CampingCore.Application.UseCases.Auth.VerifyEmail;
using CampingCore.Application.UseCases.Auth.ResendVerificationEmail;
using CampingCore.Application.Users.Commands.ForgotPassword;
using CampingCore.Application.Users.Commands.Login;
using CampingCore.Application.Users.Commands.RegisterUser;
using CampingCore.Application.Users.Commands.ResetPassword;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Registro e inicio de sesión. Login devuelve JWT de acceso (<c>token</c>) y <c>refreshToken</c> opaco para <c>POST /api/auth/refresh</c>.
/// Las llamadas API autenticadas usan <c>Authorization: Bearer {token}</c>.
/// </summary>
[Route("api/auth")]
public sealed class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender) { }

    /// <summary>
    /// Crea un usuario con contraseña hasheada y el rol indicado en <c>role</c>: <c>customer</c> o <c>admin</c> (mapeados a los roles del sistema).
    /// No devuelve JWT: usa <c>POST /api/auth/login</c> después de verificar email.
    /// </summary>
    /// <param name="command">Cuerpo con <c>name</c>, <c>email</c>, <c>password</c> y <c>role</c> (<c>customer</c> | <c>admin</c>).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> del usuario; <c>400</c> si el email ya existe, validación falla o el rol no existe en el sistema.</returns>
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
    /// Verifica el email del usuario con el código enviado por correo.
    /// </summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Email verificado correctamente.");
    }

    /// <summary>
    /// Solicita restablecer contraseña. Si el email está registrado, se envía un código con vigencia corta y un enlace al front.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Si el correo existe en nuestro sistema, recibirás instrucciones para restablecer la contraseña.");
    }

    /// <summary>
    /// Cambia la contraseña usando el código enviado por correo.
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Contraseña actualizada correctamente.");
    }

    /// <summary>
    /// Autentica por email y contraseña. Éxito: <c>userId</c>, <c>name</c>, <c>email</c>, <c>roleName</c>, <c>token</c> (JWT de acceso) y <c>refreshToken</c> (guardar para renovar sesión).
    /// </summary>
    /// <param name="command">Cuerpo con <c>email</c> y <c>password</c>.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="LoginResponse"/>; <c>401</c> si credenciales incorrectas, email no verificado (<c>User.EmailNotVerified</c>, con <c>data.userId</c>) o cuenta admin en período de activación (<c>User.AdminAccountNotReady</c>).</returns>
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
        {
            object? failData = result.Error.Code == CampingCore.Domain.Entities.User.Errors.EmailNotVerified.Code
                ? result.FailureData
                : null;

            return StatusCode(
                StatusCodes.Status401Unauthorized,
                ApiResponse.Fail(401, result.Error.Description, result.Error.Code, failData));
        }

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Renueva el JWT de acceso y rota el refresh. Body: <c>{ "refreshToken": "..." }</c>.
    /// Tras el tiempo configurado en <c>JwtSettings:RefreshIdleTimeoutDays</c> sin uso, devuelve <c>401</c> (<c>Auth.RefreshSessionIdleExpired</c>). Clientes SPA/PWA suelen llamar esto ante <c>401</c> por token caduco o al volver la app al primer plano.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshSessionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                ApiResponse.Fail(401, result.Error.Description, result.Error.Code, null));
        }

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Cierra la sesión del refresh actual (revoca el token opaco). Body: <c>{ "refreshToken": "..." }</c>.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Sesión cerrada.");
    }

    /// <summary>
    /// Reenvía el código de verificación de email si el correo existe y aún no está verificado. Respuesta siempre la misma por privacidad.
    /// </summary>
    [HttpPost("resend-verification")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromBody] ResendVerificationEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Si el correo existe y no está verificado, enviaremos un nuevo código.");
    }
}
