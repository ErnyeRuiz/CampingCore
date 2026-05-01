using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Roles.Commands.SetRolePermissions;

internal sealed class SetRolePermissionsCommandHandler : ICommandHandler<SetRolePermissionsCommand>
{
    private static readonly Error SomePermissionsNotFound =
        new("Role.PermissionsNotFound", "Uno o más identificadores de permiso no existen.");

    private readonly IRoleRepository       _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork           _unitOfWork;

    public SetRolePermissionsCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository       = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork           = unitOfWork;
    }

    public async Task<Result> Handle(SetRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(Role.Errors.NotFound);

        var distinctIds = request.PermissionIds.Distinct().ToList();

        if (distinctIds.Count > 0)
        {
            var foundPermissions = await _permissionRepository.GetByIdsAsync(distinctIds, cancellationToken);
            if (foundPermissions.Count != distinctIds.Count)
                return Result.Failure(SomePermissionsNotFound);
        }

        // Eliminar los permisos actuales del rol (la navegación fue cargada con include)
        role.RolePermissions.Clear();

        // Asignar el nuevo conjunto de permisos
        foreach (var permissionId in distinctIds)
            role.RolePermissions.Add(new RolePermission(role.Id, permissionId));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
