using CampingCore.Application.Roles;
using CampingCore.Application.Roles.Commands.CreateRole;
using CampingCore.Application.Roles.Commands.DeleteRole;
using CampingCore.Application.Roles.Commands.SetRolePermissions;
using CampingCore.Application.Roles.Commands.UpdateRole;
using CampingCore.Application.Roles.Queries.GetAllRoles;
using CampingCore.Application.Roles.Queries.GetRoleById;
using CampingCore.Application.Security;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Catálogo de roles y asignación de permisos. Solo el rol JWT <c>SuperUser</c> puede acceder a estas rutas.
/// </summary>
[Route("api/roles")]
[Authorize(Roles = AppRoles.SuperUser)]
public sealed class RolesController : ApiController
{
    public RolesController(ISender sender) : base(sender) { }

    /// <summary>
    /// Retorna todos los roles con sus permisos asociados.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllRolesQuery(), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Retorna un rol por su ID, incluidos los permisos asociados.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetRoleByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Crea un nuevo rol.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return CreatedResponse(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Actualiza nombre y descripción de un rol existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new UpdateRoleCommand(id, request.Name, request.Description), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Rol actualizado correctamente.");
    }

    /// <summary>
    /// Elimina un rol por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.SuperUser)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteRoleCommand(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Rol eliminado correctamente.");
    }

    /// <summary>
    /// Reemplaza el conjunto completo de permisos de un rol (sync/replace-all).
    /// Envía la lista final de IDs de permisos deseada; los permisos previos son eliminados.
    /// </summary>
    [HttpPut("{id:int}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPermissions(int id, [FromBody] SetRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new SetRolePermissionsCommand(id, request.PermissionIds), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Permisos del rol actualizados correctamente.");
    }
}

public record UpdateRoleRequest(string Name, string? Description);
public record SetRolePermissionsRequest(IReadOnlyList<int> PermissionIds);
