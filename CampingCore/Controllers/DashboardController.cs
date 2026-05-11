using CampingCore.Application.Dashboard.Queries.GetCampSiteStats;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Métricas públicas para paneles; no requiere JWT.
/// </summary>
[Route("api/dashboard")]
public sealed class DashboardController : ApiController
{
    public DashboardController(ISender sender) : base(sender) { }

    /// <summary>
    /// Conteo de sitios de camping y promedio de <c>Rating</c> (solo sitios con calificación &gt; 0), con filtros geográficos opcionales.
    /// </summary>
    /// <param name="idProvincia">Filtro opcional por provincia.</param>
    /// <param name="idCanton">Filtro opcional por cantón.</param>
    /// <param name="idDistrito">Filtro opcional por distrito.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con totales y promedio; <c>400</c> si algún id enviado es inválido.</returns>
    [HttpGet("camp-site-stats")]
    [ProducesResponseType(typeof(ApiResponse<CampSiteStatsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCampSiteStats(
        [FromQuery] int? idProvincia,
        [FromQuery] int? idCanton,
        [FromQuery] int? idDistrito,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetCampSiteStatsQuery(idProvincia, idCanton, idDistrito),
            cancellationToken);

        if (result.IsFailure)
            return MapErrorResponse(result.Error);

        return OkResponse(result.Value);
    }
}
