using CampingCore.Application.Abstractions.Geo;
using CampingCore.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

/// <summary>
/// Proxy hacia la API pública de divisiones territoriales de Costa Rica.
/// Permite obtener provincias, cantones y distritos para el formulario de ubicación de un sitio de camping.
/// </summary>
[Route("api/ubicacion")]
public sealed class UbicacionController : ApiController
{
    private readonly IGeoApiService _geoApi;

    public UbicacionController(ISender sender, IGeoApiService geoApi) : base(sender)
    {
        _geoApi = geoApi;
    }

    /// <summary>
    /// Retorna todas las provincias de Costa Rica.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la lista de provincias.</returns>
    [HttpGet("provincias")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProvinciaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvincias(CancellationToken cancellationToken)
    {
        var provincias = await _geoApi.GetProvinciasAsync(cancellationToken);
        return OkResponse(provincias);
    }

    /// <summary>
    /// Retorna los cantones de una provincia.
    /// </summary>
    /// <param name="idProvincia">Identificador de la provincia.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la lista de cantones.</returns>
    [HttpGet("cantones")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CantonDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCantones([FromQuery] int idProvincia, CancellationToken cancellationToken)
    {
        var cantones = await _geoApi.GetCantonesByProvinciaAsync(idProvincia, cancellationToken);
        return OkResponse(cantones);
    }

    /// <summary>
    /// Retorna los distritos de un cantón.
    /// </summary>
    /// <param name="idCanton">Identificador del cantón.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>200</c> con la lista de distritos.</returns>
    [HttpGet("distritos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DistritoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDistritos([FromQuery] int idCanton, CancellationToken cancellationToken)
    {
        var distritos = await _geoApi.GetDistritosByCantonAsync(idCanton, cancellationToken);
        return OkResponse(distritos);
    }
}
