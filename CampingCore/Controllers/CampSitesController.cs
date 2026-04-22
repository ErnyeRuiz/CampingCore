using CampingCore.Application.CampSites.Commands.CreateCampSite;
using CampingCore.Application.CampSites.Commands.DeleteCampSite;
using CampingCore.Application.CampSites.Commands.UpdateCampSite;
using CampingCore.Application.CampSites.Queries.GetAllCampSites;
using CampingCore.Application.CampSites.Queries.GetCampSiteById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

[Route("api/campsites")]
public sealed class CampSitesController : ApiController
{
    public CampSitesController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CampSiteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllCampSitesQuery(), cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CampSiteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCampSiteByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            GetCurrentUserId());

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            GetCurrentUserId());

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return MapError(result.Error);

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteCampSiteCommand(id, GetCurrentUserId()),
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

public record UpdateCampSiteRequest(
    string  Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool    HasWater,
    bool    HasElectricity);

public record CreateCampSiteRequest(
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal PricePerNight,
    bool HasWater,
    bool HasElectricity);
