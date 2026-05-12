namespace CampingCore.Application.Abstractions.Authentication;

/// <summary>
/// Genera tokens opacos de refresh y sus hashes para persistencia (SHA256 hex).
/// </summary>
public interface IRefreshTokenSecretService
{
    string GeneratePlainToken();

    string ComputeHash(string plainToken);
}
