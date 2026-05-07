using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Permissions.Commands.CreatePermission;

internal sealed class CreatePermissionCommandHandler : ICommandHandler<CreatePermissionCommand, int>
{
    private static readonly Error NameAlreadyExists =
        new("Permission.NameAlreadyExists", "Ya existe un permiso con ese nombre.");

    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork           _unitOfWork;

    public CreatePermissionCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork           = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        if (await _permissionRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return Result.Failure<int>(NameAlreadyExists);

        var result = Permission.Create(request.Name, request.Description);
        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _permissionRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
