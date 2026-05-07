using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Permissions;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Roles.Queries.GetAllRoles;

internal sealed class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, IReadOnlyList<RoleResponse>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        var response = roles.Select(role =>
        {
            var permissions = role.RolePermissions
                .Select(rp => new PermissionResponse(rp.Permission.Id, rp.Permission.Name, rp.Permission.Description, rp.Permission.CreatedAt))
                .ToList();

            return new RoleResponse(role.Id, role.Name, role.Description, role.CreatedAt, permissions);
        }).ToList();

        return response;
    }
}
