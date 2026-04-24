namespace CampingCore.Application.Abstractions.Geo;

public interface IGeoApiService
{
    Task<IReadOnlyList<ProvinciaDto>> GetProvinciasAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CantonDto>> GetCantonesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DistritoDto>> GetDistritosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CantonDto>> GetCantonesByProvinciaAsync(int idProvincia, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DistritoDto>> GetDistritosByCantonAsync(int idCanton, CancellationToken cancellationToken = default);
}
