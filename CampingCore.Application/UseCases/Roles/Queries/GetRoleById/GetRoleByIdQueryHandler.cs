using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Permissions;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Roles.Queries.GetRoleById;

internal sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleResponse>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure<RoleResponse>(Role.Errors.NotFound);

        return MapToResponse(role);
    }

    private static RoleResponse MapToResponse(Role role)
    {
        var permissions = role.RolePermissions
            .Select(rp => new PermissionResponse(rp.Permission.Id, rp.Permission.Name, rp.Permission.Description, rp.Permission.CreatedAt))
            .ToList();

        return new RoleResponse(role.Id, role.Name, role.Description, role.CreatedAt, permissions);
    }
}
