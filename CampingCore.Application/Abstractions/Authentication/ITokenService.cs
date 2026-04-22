using CampingCore.Domain.Entities;

namespace CampingCore.Application.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateToken(User user);
}
