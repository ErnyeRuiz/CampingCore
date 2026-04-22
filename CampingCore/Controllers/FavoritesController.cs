using CampingCore.Application.Favorites.Commands.AddFavorite;
using CampingCore.Application.Favorites.Commands.RemoveFavorite;
using CampingCore.Application.Favorites.Queries.GetFavoritesByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Favoritos del usuario autenticado. Todas las rutas requieren JWT; el <c>userId</c> se toma del token.
/// </summary>
[Route("api/favorites")]
[Authorize]
public sealed class FavoritesController : ApiController
{
    public FavoritesController(ISender sender) : base(sender) { }

    /// <summary>
    /// Lista los favoritos del usuario actual.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la colección.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FavoriteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetFavoritesByUserQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Añade un sitio a favoritos.
    /// </summary>
    /// <param name="campSiteId">Id del sitio a marcar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> del favorito; <c>400</c> si ya existía o datos inválidos.</returns>
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

    /// <summary>
    /// Quita un sitio de favoritos.
    /// </summary>
    /// <param name="campSiteId">Id del sitio a desmarcar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>204</c> sin cuerpo; <c>400</c> si el favorito no existía.</returns>
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
