using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender) => Sender = sender;

    protected int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.Parse(claim!);
    }
}
