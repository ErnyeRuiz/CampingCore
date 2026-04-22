using CampingCore.Application.Users.Commands.UpdateUser;
using CampingCore.Application.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingCore.Controllers;

[Route("api/users")]
[Authorize]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender) { }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUserByIdQuery(GetCurrentUserId()), cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateUserCommand(request.Name, GetCurrentUserId()),
            cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok();
    }
}

public record UpdateUserRequest(string Name);
