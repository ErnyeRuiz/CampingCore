using CampingCore.Application.Permissions;
using CampingCore.Application.Permissions.Commands.CreatePermission;
using CampingCore.Application.Permissions.Commands.DeletePermission;
using CampingCore.Application.Permissions.Commands.UpdatePermission;
using CampingCore.Application.Permissions.Queries.GetAllPermissions;
using CampingCore.Application.Permissions.Queries.GetPermissionById;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Catálogo de permisos. Requiere JWT; no hay políticas adicionales por rol más allá de un token válido.
/// </summary>
[Route("api/permissions")]
[Authorize]
public sealed class PermissionsController : ApiController
{
    public PermissionsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Retorna todos los permisos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PermissionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllPermissionsQuery(), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Retorna un permiso por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetPermissionByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Crea un nuevo permiso.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return CreatedResponse(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Actualiza nombre y descripción de un permiso existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new UpdatePermissionCommand(id, request.Name, request.Description), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Permiso actualizado correctamente.");
    }

    /// <summary>
    /// Elimina un permiso por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeletePermissionCommand(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Permiso eliminado correctamente.");
    }
}

public record UpdatePermissionRequest(string Name, string? Description);
