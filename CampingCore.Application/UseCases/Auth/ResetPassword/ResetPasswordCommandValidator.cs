using CampingCore.Application.Users.Commands.ResetPassword;
using FluentValidation;

namespace CampingCore.Application.UseCases.Auth.ResetPassword;

internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .MaximumLength(255).WithMessage("El email no puede superar 255 caracteres.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .Matches(@"^\d{6}$").WithMessage("El código debe tener 6 dígitos.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
