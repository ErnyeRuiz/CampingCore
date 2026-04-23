using CampingCore.Application.CampSites.Commands.CreateCampSite;
using CampingCore.Application.CampSites.Commands.DeleteCampSite;
using CampingCore.Application.CampSites.Commands.UpdateCampSite;
using CampingCore.Application.CampSites.Queries.GetAllCampSites;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Listado, detalle y CRUD de sitios de camping. Crear/editar/eliminar requieren JWT; el creador es <c>CreatedByUserId</c>.
/// </summary>
[Route("api/campsites")]
public sealed class CampSitesController : ApiController
{
    public CampSitesController(ISender sender) : base(sender) { }

    /// <summary>
    /// Lista todos los sitios de camping.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la colección.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CampSiteResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllCampSitesQuery(), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Obtiene un sitio de camping por id.
    /// </summary>
    /// <param name="id">Identificador del sitio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="CampSiteResponse"/>; <c>404</c> si no existe.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CampSiteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCampSiteByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }

    /// <summary>
    /// Crea un sitio de camping. El creador queda fijado al usuario del JWT.
    /// </summary>
    /// <param name="request">Datos del sitio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c>; <c>400</c> en error de validación o dominio.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCampSiteRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCampSiteCommand(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.HasWater,
            request.HasElectricity,
            GetCurrentUserId(),
            request.IdProvincia,
            request.IdCanton,
            request.IdDistrito,
            request.DireccionExacta);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return CreatedResponse(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Actualiza un sitio. Solo el creador (<c>CreatedByUserId</c>) puede modificar.
    /// </summary>
    /// <param name="id">Identificador del sitio.</param>
    /// <param name="request">Nuevos datos.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>403</c> si no es el dueño; <c>404</c> no encontrado.</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCampSiteRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCampSiteCommand(
            id,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.HasWater,
            request.HasElectricity,
            GetCurrentUserId(),
            request.IdProvincia,
            request.IdCanton,
            request.IdDistrito,
            request.DireccionExacta);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Sitio de camping actualizado exitosamente.");
    }

    /// <summary>
    /// Elimina un sitio. Solo el creador puede eliminar.
    /// </summary>
    /// <param name="id">Identificador del sitio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>403</c> si no es el dueño; <c>404</c> no encontrado.</returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteCampSiteCommand(id, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Sitio de camping eliminado exitosamente.");
    }
}

/// <param name="Name">Nombre del sitio.</param>
/// <param name="Description">Descripción opcional.</param>
/// <param name="Latitude">Latitud (-90 a 90).</param>
/// <param name="Longitude">Longitud (-180 a 180).</param>
/// <param name="PricePerNight">Precio por noche (mayor que 0).</param>
/// <param name="HasWater">Indica agua en el sitio.</param>
/// <param name="HasElectricity">Indica electricidad en el sitio.</param>
/// <param name="IdProvincia">Identificador de provincia.</param>
/// <param name="IdCanton">Identificador de cantón.</param>
/// <param name="IdDistrito">Identificador de distrito.</param>
/// <param name="DireccionExacta">Otras señas / dirección exacta (opcional).</param>
public record UpdateCampSiteRequest(
    string  Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool    HasWater,
    bool    HasElectricity,
    int     IdProvincia,
    int     IdCanton,
    int     IdDistrito,
    string? DireccionExacta);

/// <param name="Name">Nombre del sitio.</param>
/// <param name="Description">Descripción opcional.</param>
/// <param name="Latitude">Latitud (-90 a 90).</param>
/// <param name="Longitude">Longitud (-180 a 180).</param>
/// <param name="PricePerNight">Precio por noche (mayor que 0).</param>
/// <param name="HasWater">Indica agua en el sitio.</param>
/// <param name="HasElectricity">Indica electricidad en el sitio.</param>
/// <param name="IdProvincia">Identificador de provincia.</param>
/// <param name="IdCanton">Identificador de cantón.</param>
/// <param name="IdDistrito">Identificador de distrito.</param>
/// <param name="DireccionExacta">Otras señas / dirección exacta (opcional).</param>
public record CreateCampSiteRequest(
    string  Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool    HasWater,
    bool    HasElectricity,
    int     IdProvincia,
    int     IdCanton,
    int     IdDistrito,
    string? DireccionExacta);
