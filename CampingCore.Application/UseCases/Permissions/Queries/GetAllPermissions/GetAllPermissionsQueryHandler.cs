using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Permissions.Queries.GetAllPermissions;

internal sealed class GetAllPermissionsQueryHandler : IQueryHandler<GetAllPermissionsQuery, IReadOnlyList<PermissionResponse>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<IReadOnlyList<PermissionResponse>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        var response = permissions
            .Select(p => new PermissionResponse(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToList();

        return response;
    }
}
