using FluentValidation;

namespace CampingCore.Application.UseCases.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
    }
}
