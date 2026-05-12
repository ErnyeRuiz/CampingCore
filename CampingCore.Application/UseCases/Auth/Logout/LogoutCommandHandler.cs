using CampingCore.Application.Abstractions.Authentication;
using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.UseCases.Auth.Logout;

internal sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository    _refreshTokens;
    private readonly IRefreshTokenSecretService _secrets;
    private readonly IUnitOfWork                _unitOfWork;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokens,
        IRefreshTokenSecretService secrets,
        IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _secrets       = secrets;
        _unitOfWork    = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var hash = _secrets.ComputeHash(request.RefreshToken);
        var row  = await _refreshTokens.GetActiveByHashWithUserAsync(hash, cancellationToken);

        if (row is null)
            return Result.Success();

        row.Revoke(DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
