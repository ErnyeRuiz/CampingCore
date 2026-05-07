using CampingCore.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace CampingCore.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.IsInRole(AppRoles.SuperUser))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        foreach (var claim in context.User.FindAll("permission"))
        {
            if (claim.Value == requirement.PermissionName)
            {
                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
