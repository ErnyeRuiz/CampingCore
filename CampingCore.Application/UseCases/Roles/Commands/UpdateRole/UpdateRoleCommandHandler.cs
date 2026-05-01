using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Roles.Commands.UpdateRole;

internal sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private static readonly Error NameAlreadyExists =
        new("Role.NameAlreadyExists", "Ya existe un rol con ese nombre.");

    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure(Role.Errors.NotFound);

        if (role.Name != request.Name &&
            await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return Result.Failure(NameAlreadyExists);

        var updateResult = role.Update(request.Name, request.Description);
        if (updateResult.IsFailure)
            return updateResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
