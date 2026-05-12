using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Users.Commands.ResetPassword;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.UseCases.Auth.ResetPassword;

internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokens  = refreshTokens;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Failure(User.Errors.InvalidOrExpiredVerificationCode);

        if (string.IsNullOrEmpty(user.EmailVerificationCode)
            || user.EmailVerificationCodeExpiresAt is null
            || DateTime.UtcNow > user.EmailVerificationCodeExpiresAt.Value)
            return Result.Failure(User.Errors.InvalidOrExpiredVerificationCode);

        if (!BCrypt.Net.BCrypt.Verify(request.Code, user.EmailVerificationCode))
            return Result.Failure(User.Errors.InvalidOrExpiredVerificationCode);

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        var reset = user.ResetPassword(newHash);

        if (reset.IsFailure)
            return reset;

        var utcNow = DateTime.UtcNow;
        var sessions = await _refreshTokens.GetActiveByUserIdAsync(user.Id, cancellationToken);
        foreach (var session in sessions)
            session.Revoke(utcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
