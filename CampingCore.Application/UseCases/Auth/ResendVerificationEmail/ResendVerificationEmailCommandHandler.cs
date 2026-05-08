using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.UseCases.Auth.ResendVerificationEmail;

internal sealed class ResendVerificationEmailCommandHandler : ICommandHandler<ResendVerificationEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork     _unitOfWork;

    public ResendVerificationEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || user.IsEmailVerified)
            return Result.Success();

        var plainCode = Random.Shared.Next(100000, 1000000).ToString();
        user.GenerateEmailVerificationCode(plainCode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
