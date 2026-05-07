using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Application.Security;
using Microsoft.AspNetCore.Http;

namespace CampingCore.Infrastructure.Authentication;

internal sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.Parse(claim!);
        }
    }

    public bool IsSuperUser =>
        _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.SuperUser) ?? false;
}
