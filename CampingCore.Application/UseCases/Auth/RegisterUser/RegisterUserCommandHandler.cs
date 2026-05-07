using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Users.Commands.RegisterUser;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using System.Diagnostics;

namespace CampingCore.Application.UseCases.Auth.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, int>
{
    private static readonly Error CustomerRoleNotFound =
        new("User.CustomerRoleNotFound", "El rol 'Customer' no existe en el sistema. Contacte al administrador.");

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result<int>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure<int>(new Error("User.EmailAlreadyExists", $"El email '{request.Email}' ya está registrado."));

        var customerRole = await _roleRepository.GetByNameAsync("Customer", cancellationToken);
        if (customerRole is null)
            return Result.Failure<int>(CustomerRoleNotFound);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var result = User.Create(request.Name, request.Email, passwordHash, customerRole.Id);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var user = result.Value;
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        user.GenerateEmailVerificationCode(Random.Shared.Next(100000, 999999).ToString());

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
