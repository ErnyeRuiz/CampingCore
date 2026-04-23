using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CampingCore.Application.Abstractions.Geo;

namespace CampingCore.Infrastructure.ExternalServices;

internal sealed class GeoApiService : IGeoApiService
{
    private readonly HttpClient _httpClient;

    public GeoApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ProvinciaDto>> GetProvinciasAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<GeoApiResponse<ProvinciaResponse>>(
            "provincias", cancellationToken);

        return response?.Data.Select(p => new ProvinciaDto(p.IdProvincia, p.Descripcion)).ToList()
            ?? [];
    }

    public async Task<IReadOnlyList<CantonDto>> GetCantonesByProvinciaAsync(int idProvincia, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<GeoApiResponse<CantonResponse>>(
            $"cantones?idProvincia={idProvincia}", cancellationToken);

        return response?.Data.Select(c => new CantonDto(c.IdCanton, c.IdProvincia, c.Descripcion)).ToList()
            ?? [];
    }

    public async Task<IReadOnlyList<DistritoDto>> GetDistritosByCantonAsync(int idCanton, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<GeoApiResponse<DistritoResponse>>(
            $"distritos?idCanton={idCanton}", cancellationToken);

        return response?.Data.Select(d => new DistritoDto(d.IdDistrito, d.IdCanton, d.Descripcion)).ToList()
            ?? [];
    }

    private sealed class GeoApiResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; init; } = [];
    }

    private sealed class ProvinciaResponse
    {
        [JsonPropertyName("idProvincia")]
        public int IdProvincia { get; init; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; init; } = string.Empty;
    }

    private sealed class CantonResponse
    {
        [JsonPropertyName("idCanton")]
        public int IdCanton { get; init; }

        [JsonPropertyName("idProvincia")]
        public int IdProvincia { get; init; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; init; } = string.Empty;
    }

    private sealed class DistritoResponse
    {
        [JsonPropertyName("idDistrito")]
        public int IdDistrito { get; init; }

        [JsonPropertyName("idCanton")]
        public int IdCanton { get; init; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; init; } = string.Empty;
    }
}
