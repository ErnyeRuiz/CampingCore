using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Permissions.Queries.GetPermissionById;

internal sealed class GetPermissionByIdQueryHandler : IQueryHandler<GetPermissionByIdQuery, PermissionResponse>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<PermissionResponse>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission is null)
            return Result.Failure<PermissionResponse>(Permission.Errors.NotFound);

        return new PermissionResponse(permission.Id, permission.Name, permission.Description, permission.CreatedAt);
    }
}
