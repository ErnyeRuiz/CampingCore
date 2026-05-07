using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Users.Commands.UpdateUserByAdmin;

internal sealed class UpdateUserByAdminCommandHandler : ICommandHandler<UpdateUserByAdminCommand>
{
    private static readonly Error EmailAlreadyExists =
        Error.Validation("User.EmailAlreadyExists", "Ya existe otro usuario con ese correo electrónico.");

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserByAdminCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserByAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound(nameof(User), request.UserId));

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(Error.NotFound(nameof(Role), request.RoleId));

        var email = request.Email.Trim();

        if (await _userRepository.ExistsByEmailExceptUserIdAsync(email, request.UserId, cancellationToken))
            return Result.Failure(EmailAlreadyExists);

        string? passwordHash = string.IsNullOrWhiteSpace(request.Password)
            ? null
            : BCrypt.Net.BCrypt.HashPassword(request.Password);

        var update = user.UpdateByAdmin(request.Name, email, request.RoleId, passwordHash);

        if (update.IsFailure)
            return update;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
