using CampingCore.Application.Users.Commands.ForgotPassword;
using FluentValidation;

namespace CampingCore.Application.UseCases.Auth.ForgotPassword;

internal sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .MaximumLength(255).WithMessage("El email no puede superar 255 caracteres.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");
    }
}
