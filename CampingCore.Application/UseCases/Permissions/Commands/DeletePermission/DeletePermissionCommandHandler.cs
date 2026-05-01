using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Permissions.Commands.DeletePermission;

internal sealed class DeletePermissionCommandHandler : ICommandHandler<DeletePermissionCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork           _unitOfWork;

    public DeletePermissionCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork           = unitOfWork;
    }

    public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission is null)
            return Result.Failure(Permission.Errors.NotFound);

        _permissionRepository.Remove(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
