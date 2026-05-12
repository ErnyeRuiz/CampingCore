using FluentValidation;

namespace CampingCore.Application.UseCases.Auth.RefreshSession;

public sealed class RefreshSessionCommandValidator : AbstractValidator<RefreshSessionCommand>
{
    public RefreshSessionCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("El token de renovación es obligatorio.");
    }
}
