using FluentValidation;

namespace CampingCore.Application.Users.Commands.UpdateUserByAdmin;

internal sealed class UpdateUserByAdminCommandValidator : AbstractValidator<UpdateUserByAdminCommand>
{
    public UpdateUserByAdminCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El identificador de usuario debe ser mayor a 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .MaximumLength(255).WithMessage("El email no puede superar 255 caracteres.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("El identificador de rol debe ser mayor a 0.");

        RuleFor(x => x.Password)
            .Must(p => p is null || p.Length >= 8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres si se envía.")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}
