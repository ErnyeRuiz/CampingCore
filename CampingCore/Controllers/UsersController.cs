using CampingCore.Application.Security;
using CampingCore.Application.Users.Commands.UpdateUser;
using CampingCore.Application.Users.Commands.UpdateUserByAdmin;
using CampingCore.Application.Users.Queries.GetSystemUsersList;
using CampingCore.Application.Users.Queries.GetUserAdminById;
using CampingCore.Application.Users.Queries.GetUserById;
using CampingCore.Common;
using CampingCore.Domain.ReadModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Perfil del usuario autenticado (<c>GET/PUT /api/users/me</c>). <c>GET /api/users</c> (lista), <c>GET/PUT /api/users/{id}</c> cargar y editar usuario: solo rol <c>SuperUser</c>.
/// </summary>
[Route("api/users")]
[Authorize]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender) { }

    /// <summary>
    /// Lista todos los usuarios con rol, fecha de alta y conteos de campings creados, viajes y favoritos. Requiere JWT con rol <c>SuperUser</c>.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la colección; <c>403</c> si el rol no es <c>SuperUser</c>.</returns>
    [HttpGet]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserSystemListItem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllSystemUsers(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetSystemUsersListQuery(), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Obtiene un usuario por id (id, name, email, createdAt, roleId, roleName). Solo rol <c>SuperUser</c>.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con el detalle; <c>404</c> si no existe; <c>403</c> sin <c>SuperUser</c>.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<UserAdminDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdForAdmin(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUserAdminByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Actualiza un usuario: nombre, email (único), rol y contraseña opcional (si no se envía o va vacía, se mantiene). Solo rol <c>SuperUser</c>.
    /// </summary>
    /// <param name="id">Identificador del usuario a modificar.</param>
    /// <param name="request">Datos actualizados. <c>password</c> omitir o <c>null</c> para no cambiar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>400</c> validación o email duplicado; <c>404</c> usuario o rol; <c>403</c> sin <c>SuperUser</c>.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateByAdmin(
        int id,
        [FromBody] UpdateUserByAdminRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateUserByAdminCommand(id, request.Name, request.Email, request.RoleId, request.Password),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Usuario actualizado exitosamente.");
    }

    /// <summary>
    /// Devuelve el perfil del usuario cuyo id coincide con el claim <c>sub</c> del JWT.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="UserResponse"/> (<c>id</c>, <c>name</c>, <c>email</c>, <c>createdAt</c>, <c>roleName</c>); <c>404</c> si no hay coincidencia con el token.</returns>
    [HttpGet("me")]
    [Authorize]
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
    [Authorize]
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

/// <param name="Name">Nombre completo o visible (máx. 100 caracteres).</param>
/// <param name="Email">Correo único en el sistema (salvo el propio usuario).</param>
/// <param name="RoleId">Identificador del rol en la tabla <c>Roles</c>.</param>
/// <param name="Password">Nueva contraseña (mín. 8 caracteres). Omitir o <c>null</c> para no cambiar.</param>
public record UpdateUserByAdminRequest(string Name, string Email, int RoleId, string? Password);
