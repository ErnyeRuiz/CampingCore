using CampingCore.Application.Reviews.Commands.CreateReview;
using CampingCore.Application.Reviews.Commands.DeleteReview;
using CampingCore.Application.Reviews.Commands.UpdateReview;
using CampingCore.Application.Reviews.Queries.GetReviewsByCampSite;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Reseñas por sitio bajo <c>api/campsites/{campSiteId}/reviews</c>.
/// Las operaciones de edición/eliminación usan rutas absolutas <c>PUT/DELETE /api/reviews/{id}</c> (mismo controlador).
/// </summary>
[Route("api/campsites/{campSiteId:int}/reviews")]
public sealed class ReviewsController : ApiController
{
    public ReviewsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Lista reseñas de un sitio de camping.
    /// </summary>
    /// <param name="campSiteId">Id del sitio (en la ruta del controlador).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la lista.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ReviewResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCampSite(int campSiteId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetReviewsByCampSiteQuery(campSiteId), cancellationToken);
        return OkResponse(result.Value);
    }

    /// <summary>
    /// Crea una reseña. El autor es el usuario del JWT; el <c>campSiteId</c> va en la ruta.
    /// </summary>
    /// <param name="campSiteId">Id del sitio reseñado.</param>
    /// <param name="request">Calificación y comentario opcional.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>201</c> con <c>id</c> de la reseña; <c>400</c> en error de validación o dominio.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
            return MapErrorResponse(result.Error);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Success(new { id = result.Value }, 201, "Reseña creada exitosamente."));
    }

    /// <summary>
    /// Actualiza una reseña existente. Ruta: <c>PUT /api/reviews/{id}</c>.
    /// </summary>
    /// <param name="id">Id de la reseña.</param>
    /// <param name="request">Nueva calificación y comentario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>403</c> si no es el autor; <c>404</c> no encontrada.</returns>
    [HttpPut("~/api/reviews/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateReviewCommand(id, request.Rating, request.Comment, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Reseña actualizada exitosamente.");
    }

    /// <summary>
    /// Elimina una reseña. Ruta: <c>DELETE /api/reviews/{id}</c>. Solo el autor.
    /// </summary>
    /// <param name="id">Id de la reseña.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con envelope; <c>403</c> si no es el autor; <c>404</c> no encontrada.</returns>
    [HttpDelete("~/api/reviews/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new DeleteReviewCommand(id, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return SuccessResponse("Reseña eliminada exitosamente.");
    }
}

/// <param name="Rating">Puntuación de 1 a 5.</param>
/// <param name="Comment">Comentario opcional.</param>
public record UpdateReviewRequest(byte Rating, string? Comment);

/// <param name="Rating">Puntuación de 1 a 5.</param>
/// <param name="Comment">Comentario opcional.</param>
public record CreateReviewRequest(byte Rating, string? Comment);
