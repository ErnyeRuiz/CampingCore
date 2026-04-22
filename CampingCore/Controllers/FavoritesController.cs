using CampingCore.Application.Favorites.Commands.AddFavorite;
using CampingCore.Application.Favorites.Commands.RemoveFavorite;
using CampingCore.Application.Favorites.Queries.GetFavoritesByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

[Route("api/favorites")]
[Authorize]
public sealed class FavoritesController : ApiController
{
    public FavoritesController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FavoriteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetFavoritesByUserQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("{campSiteId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new AddFavoriteCommand(GetCurrentUserId(), campSiteId),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created(string.Empty, new { id = result.Value });
    }

    [HttpDelete("{campSiteId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Remove(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RemoveFavoriteCommand(GetCurrentUserId(), campSiteId),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}
