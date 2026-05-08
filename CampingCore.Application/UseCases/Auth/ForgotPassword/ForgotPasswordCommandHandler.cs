using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Users.Commands.ForgotPassword;
using CampingCore.Domain.Common;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.UseCases.Auth.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Success();

        user.GeneratePasswordResetCode(Random.Shared.Next(100000, 1000000).ToString());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
