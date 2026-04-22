using CampingCore.Application.Abstractions.Authentication;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "El email o la contraseña son incorrectos.");

    private readonly IUserRepository _userRepository;
    private readonly ITokenService   _tokenService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService   = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(InvalidCredentials);

        var token = _tokenService.GenerateToken(user);

        return new LoginResponse(user.Id, user.Name, user.Email, token);
    }
}
