using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Roles.Commands.CreateRole;

internal sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, int>
{
    private static readonly Error NameAlreadyExists =
        new("Role.NameAlreadyExists", "Ya existe un rol con ese nombre.");

    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return Result.Failure<int>(NameAlreadyExists);

        var result = Role.Create(request.Name, request.Description);
        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _roleRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
