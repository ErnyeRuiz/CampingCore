using System.Security.Cryptography;
using System.Text;
using CampingCore.Application.Abstractions.Authentication;

namespace CampingCore.Infrastructure.Authentication;

internal sealed class RefreshTokenSecretService : IRefreshTokenSecretService
{
    public string GeneratePlainToken()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string ComputeHash(string plainToken)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(plainToken));
        return Convert.ToHexString(digest).ToLowerInvariant();
    }
}
