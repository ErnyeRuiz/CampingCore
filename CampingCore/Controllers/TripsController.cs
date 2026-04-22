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

[Route("api/trips")]
[Authorize]
public sealed class TripsController : ApiController
{
    public TripsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTrips(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetTripsByUserQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result.Value);
    }

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

public record UpdateTripRequest(string Name, DateOnly StartDate, DateOnly EndDate);

public record CreateTripRequest(string Name, DateOnly StartDate, DateOnly EndDate);
