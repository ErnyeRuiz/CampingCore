using CampingCore.Application.CampSites.Commands.DeleteCampSite;
using CampingCore.Application.CampSites.Queries.GetAllCampSites;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;
using CampingCore.Application.CampSites;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CampingCore.Application.UseCases.CampSites.Commands.CreateCampSite;
using CampingCore.Application.UseCases.CampSites.Commands.UpdateCampSite;

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
    /// Cuerpo <c>multipart/form-data</c>: campos escalares + colección de archivos <c>images</c> (opcional).
    /// </summary>
    /// <param name="form">Datos del sitio y archivos.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c>; <c>400</c> en error de validación o dominio.</returns>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromForm] CreateCampSiteForm form,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ImageFileDto> newImages =
            await CampSiteImageFormMapper.ToImageFileDtosAsync(form.Images, cancellationToken);

        var command = new CreateCampSiteCommand(
            form.Name,
            form.Description,
            form.Latitude,
            form.Longitude,
            form.PricePerNight,
            form.HasWater,
            form.HasElectricity,
            GetCurrentUserId(),
            form.IdProvincia,
            form.IdCanton,
            form.IdDistrito,
            form.DireccionExacta,
            newImages);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return CreatedResponse(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Actualiza un sitio. Solo el creador (<c>CreatedByUserId</c>) puede modificar.
    /// <c>multipart/form-data</c>: campos escalares + <c>images</c> (nuevas) + <c>imageIdsToKeep</c> (ids existentes a conservar; vacío = borrar todas las actuales).
    /// </summary>
    /// <param name="id">Identificador del sitio.</param>
    /// <param name="form">Nuevos datos e imágenes.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>403</c> si no es el dueño; <c>404</c> no encontrado.</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateCampSiteForm form,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ImageFileDto> newImages =
            await CampSiteImageFormMapper.ToImageFileDtosAsync(form.Images, cancellationToken);

        var command = new UpdateCampSiteCommand(
            id,
            form.Name,
            form.Description,
            form.Latitude,
            form.Longitude,
            form.PricePerNight,
            form.HasWater,
            form.HasElectricity,
            GetCurrentUserId(),
            form.IdProvincia,
            form.IdCanton,
            form.IdDistrito,
            form.DireccionExacta,
            form.ImageIdsToKeep,
            newImages);

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
