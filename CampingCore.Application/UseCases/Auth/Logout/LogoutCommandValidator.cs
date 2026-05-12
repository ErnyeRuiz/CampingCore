using FluentValidation;

namespace CampingCore.Application.UseCases.Auth.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("El token de renovación es obligatorio.");
    }
}
