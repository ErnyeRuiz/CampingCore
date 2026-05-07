using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.UseCases.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound(nameof(User), request.UserId));

        var verification = user.VerifyEmail(request.Code);
        if (verification.IsFailure)
            return verification;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
