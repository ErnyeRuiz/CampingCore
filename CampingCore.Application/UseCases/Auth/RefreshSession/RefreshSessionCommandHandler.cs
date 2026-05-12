using CampingCore.Application.Abstractions.Authentication;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Options;
using CampingCore.Application.Security;
using CampingCore.Application.Users.Commands.Login;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace CampingCore.Application.UseCases.Auth.RefreshSession;

internal sealed class RefreshSessionCommandHandler : ICommandHandler<RefreshSessionCommand, LoginResponse>
{
    private static readonly Error InvalidRefreshSession =
        Error.Validation("Auth.InvalidRefreshSession", "La sesión no es válida. Inicia sesión nuevamente.");

    private static readonly Error RefreshIdleExpired =
        Error.Validation(
            "Auth.RefreshSessionIdleExpired",
            "Tu sesión expiró por inactividad. Inicia sesión nuevamente.");

    private readonly IRefreshTokenRepository       _refreshTokens;
    private readonly IRefreshTokenSecretService    _secrets;
    private readonly ITokenService                 _tokenService;
    private readonly IUnitOfWork                   _unitOfWork;
    private readonly SessionAuthSettings           _sessionSettings;
    private readonly AdminSettings                 _adminSettings;

    public RefreshSessionCommandHandler(
        IRefreshTokenRepository refreshTokens,
        IRefreshTokenSecretService secrets,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IOptions<SessionAuthSettings> sessionSettings,
        IOptions<AdminSettings> adminSettings)
    {
        _refreshTokens      = refreshTokens;
        _secrets            = secrets;
        _tokenService       = tokenService;
        _unitOfWork         = unitOfWork;
        _sessionSettings    = sessionSettings.Value;
        _adminSettings      = adminSettings.Value;
    }

    public Task<Result<LoginResponse>> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        return _unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            var hash = _secrets.ComputeHash(request.RefreshToken);
            var row = await _refreshTokens.GetActiveByHashWithUserAsync(hash, cancellationToken);

            if (row is null)
                return Result.Failure<LoginResponse>(InvalidRefreshSession);

            var idleLimit = TimeSpan.FromDays(Math.Max(1, _sessionSettings.RefreshIdleTimeoutDays));
            if (utcNow - row.LastUsedAtUtc > idleLimit)
            {
                row.Revoke(utcNow);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Failure<LoginResponse>(RefreshIdleExpired);
            }

            var user = row.User;
            if (user is null || !user.IsEmailVerified)
                return Result.Failure<LoginResponse>(InvalidRefreshSession);

            if (string.Equals(user.Role?.Name, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                var readyAt = user.CreatedAt.AddHours(_adminSettings.ActivationDelayHours);
                if (readyAt > utcNow)
                {
                    var remainingSeconds = (int)(readyAt - utcNow).TotalSeconds;
                    return Result.Failure<LoginResponse>(User.Errors.AdminAccountNotReady(remainingSeconds));
                }
            }

            row.Revoke(utcNow);

            var plainRefresh = _secrets.GeneratePlainToken();
            var newHash      = _secrets.ComputeHash(plainRefresh);
            var freshRow     = RefreshToken.Create(user.Id, newHash, utcNow);
            _refreshTokens.Add(freshRow);

            var access = _tokenService.GenerateToken(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new LoginResponse(user.Id, user.Name, user.Email, user.Role?.Name, access, plainRefresh));
        }, cancellationToken);
    }
}
