using CampingCore.Application.Favorites.Commands.AddFavorite;
using CampingCore.Application.Favorites.Commands.RemoveFavorite;
using CampingCore.Application.Favorites.Queries.GetFavoritesByUser;
using CampingCore.Common;
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
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FavoriteResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetFavoritesByUserQuery(GetCurrentUserId()), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Añade un sitio a favoritos.
    /// </summary>
    /// <param name="campSiteId">Id del sitio a marcar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> del favorito; <c>400</c> si ya existía o datos inválidos.</returns>
    [HttpPost("{campSiteId:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new AddFavoriteCommand(GetCurrentUserId(), campSiteId),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Success(new { id = result.Value }, 201, "Favorito añadido exitosamente."));
    }

    /// <summary>
    /// Quita un sitio de favoritos.
    /// </summary>
    /// <param name="campSiteId">Id del sitio a desmarcar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>400</c> si el favorito no existía.</returns>
    [HttpDelete("{campSiteId:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Remove(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RemoveFavoriteCommand(GetCurrentUserId(), campSiteId),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Favorito eliminado exitosamente.");
    }
}
