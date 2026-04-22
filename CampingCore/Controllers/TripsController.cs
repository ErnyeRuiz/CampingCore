using CampingCore.Application.Trips.Commands.AddCampSiteToTrip;
using CampingCore.Application.Trips.Commands.CreateTrip;
using CampingCore.Application.Trips.Commands.DeleteTrip;
using CampingCore.Application.Trips.Commands.RemoveCampSiteFromTrip;
using CampingCore.Application.Trips.Commands.UpdateTrip;
using CampingCore.Application.Trips.Queries.GetTripById;
using CampingCore.Application.Trips.Queries.GetTripsByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Viajes e itinerario (inclusión de campings en un viaje). Requiere JWT; el propietario del viaje es el usuario del token.
/// </summary>
[Route("api/trips")]
[Authorize]
public sealed class TripsController : ApiController
{
    public TripsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Lista los viajes del usuario autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la colección.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTrips(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetTripsByUserQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene un viaje por id (detalle, incl. campings vinculados según la query de aplicación).
    /// </summary>
    /// <param name="id">Id del viaje.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con <see cref="TripResponse"/>; <c>404</c> si no existe.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetTripByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un viaje para el usuario del JWT.
    /// </summary>
    /// <param name="request">Nombre y rango de fechas (fin &gt;= inicio).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c>; <c>400</c> en error de validación o dominio.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTripRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTripCommand(
            GetCurrentUserId(),
            request.Name,
            request.StartDate,
            request.EndDate);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Añade un sitio al itinerario del viaje.
    /// </summary>
    /// <param name="tripId">Id del viaje.</param>
    /// <param name="campSiteId">Id del sitio a añadir.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con id de la fila de unión; <c>400</c> en conflicto o validación.</returns>
    [HttpPost("{tripId:int}/campsites/{campSiteId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCampSite(
        int tripId,
        int campSiteId,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new AddCampSiteToTripCommand(tripId, campSiteId),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created(string.Empty, new { id = result.Value });
    }

    /// <summary>
    /// Actualiza nombre y fechas del viaje. Solo el dueño.
    /// </summary>
    /// <param name="id">Id del viaje.</param>
    /// <param name="request">Nuevos datos.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> sin cuerpo; <c>403</c> no propietario; <c>404</c> no encontrado.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTripRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateTripCommand(id, request.Name, request.StartDate, request.EndDate, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapError(result.Error);

        return Ok();
    }

    /// <summary>
    /// Elimina un viaje completo. Solo el dueño.
    /// </summary>
    /// <param name="id">Id del viaje.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>204</c> sin cuerpo; <c>403</c> o <c>404</c>.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteTripCommand(id, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Quita un sitio del itinerario. Solo el dueño del viaje.
    /// </summary>
    /// <param name="tripId">Id del viaje.</param>
    /// <param name="campSiteId">Id del sitio a quitar del viaje.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>204</c> sin cuerpo; <c>400</c> si el enlace no existía; <c>403/404</c> según caso.</returns>
    [HttpDelete("{tripId:int}/campsites/{campSiteId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCampSite(
        int tripId,
        int campSiteId,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RemoveCampSiteFromTripCommand(tripId, campSiteId, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapError(result.Error);

        return NoContent();
    }

    private IActionResult MapError(Domain.Common.Error error) => error.Code switch
    {
        var c when c.EndsWith(".Forbidden") => Forbid(),
        var c when c.EndsWith(".NotFound")  => NotFound(error),
        _                                   => BadRequest(error)
    };
}

/// <param name="Name">Nombre del viaje.</param>
/// <param name="StartDate">Fecha de inicio (ISO-8601 date).</param>
/// <param name="EndDate">Fecha de fin (debe ser &gt;= inicio).</param>
public record UpdateTripRequest(string Name, DateOnly StartDate, DateOnly EndDate);

/// <param name="Name">Nombre del viaje.</param>
/// <param name="StartDate">Fecha de inicio (ISO-8601 date).</param>
/// <param name="EndDate">Fecha de fin (debe ser &gt;= inicio).</param>
public record CreateTripRequest(string Name, DateOnly StartDate, DateOnly EndDate);
