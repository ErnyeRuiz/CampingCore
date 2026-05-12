using CampingCore.Application.Abstractions.Authentication;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Options;
using CampingCore.Application.Security;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace CampingCore.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "El email o la contraseña son incorrectos.");

    private readonly IUserRepository             _userRepository;
    private readonly ITokenService               _tokenService;
    private readonly IRefreshTokenRepository    _refreshTokens;
    private readonly IRefreshTokenSecretService _secrets;
    private readonly IUnitOfWork                _unitOfWork;
    private readonly AdminSettings               _adminSettings;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokens,
        IRefreshTokenSecretService secrets,
        IUnitOfWork unitOfWork,
        IOptions<AdminSettings> adminSettings)
    {
        _userRepository = userRepository;
        _tokenService   = tokenService;
        _refreshTokens  = refreshTokens;
        _secrets        = secrets;
        _unitOfWork     = unitOfWork;
        _adminSettings  = adminSettings.Value;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailWithRoleAsync(request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(InvalidCredentials);

        if (!user.IsEmailVerified)
            return Result.Failure<LoginResponse>(
                User.Errors.EmailNotVerified,
                new { userId = user.Id });

        if (string.Equals(user.Role?.Name, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            var readyAt = user.CreatedAt.AddHours(_adminSettings.ActivationDelayHours);
            if (readyAt > DateTime.UtcNow)
            {
                var remainingSeconds = (int)(readyAt - DateTime.UtcNow).TotalSeconds;
                return Result.Failure<LoginResponse>(User.Errors.AdminAccountNotReady(remainingSeconds));
            }
        }

        var utcNow       = DateTime.UtcNow;
        var accessToken  = _tokenService.GenerateToken(user);
        var plainRefresh = _secrets.GeneratePlainToken();
        var refreshHash  = _secrets.ComputeHash(plainRefresh);
        var refreshRow   = RefreshToken.Create(user.Id, refreshHash, utcNow);

        _refreshTokens.Add(refreshRow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(user.Id, user.Name, user.Email, user.Role?.Name, accessToken, plainRefresh);
    }
}
