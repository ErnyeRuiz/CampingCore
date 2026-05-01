using FluentValidation;

namespace CampingCore.Application.Roles.Commands.CreateRole;

internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del rol no puede superar 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("La descripción no puede superar 500 caracteres.");
    }
}
