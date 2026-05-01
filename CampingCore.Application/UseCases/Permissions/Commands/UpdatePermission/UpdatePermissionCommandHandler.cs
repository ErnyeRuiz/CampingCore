using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Permissions.Commands.UpdatePermission;

internal sealed class UpdatePermissionCommandHandler : ICommandHandler<UpdatePermissionCommand>
{
    private static readonly Error NameAlreadyExists =
        new("Permission.NameAlreadyExists", "Ya existe un permiso con ese nombre.");

    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork           _unitOfWork;

    public UpdatePermissionCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork           = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission is null)
            return Result.Failure(Permission.Errors.NotFound);

        if (permission.Name != request.Name &&
            await _permissionRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return Result.Failure(NameAlreadyExists);

        var updateResult = permission.Update(request.Name, request.Description);
        if (updateResult.IsFailure)
            return updateResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
