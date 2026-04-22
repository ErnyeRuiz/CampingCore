using CampingCore.Application.Reviews.Commands.CreateReview;
using CampingCore.Application.Reviews.Commands.DeleteReview;
using CampingCore.Application.Reviews.Commands.UpdateReview;
using CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

[Route("api/campsites/{campSiteId:int}/reviews")]
public sealed class ReviewsController : ApiController
{
    public ReviewsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCampSite(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetReviewsByCampSiteQuery(campSiteId), cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        int campSiteId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(
            GetCurrentUserId(),
            campSiteId,
            request.Rating,
            request.Comment);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created(string.Empty, new { id = result.Value });
    }

    [HttpPut("~/api/reviews/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateReviewCommand(id, request.Rating, request.Comment, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapError(result.Error);

        return Ok();
    }

    [HttpDelete("~/api/reviews/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteReviewCommand(id, GetCurrentUserId()),
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

public record UpdateReviewRequest(byte Rating, string? Comment);

public record CreateReviewRequest(byte Rating, string? Comment);
