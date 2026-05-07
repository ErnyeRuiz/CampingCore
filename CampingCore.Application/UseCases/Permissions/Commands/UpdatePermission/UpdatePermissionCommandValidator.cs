using FluentValidation;

namespace CampingCore.Application.Permissions.Commands.UpdatePermission;

internal sealed class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El identificador del permiso debe ser mayor a 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del permiso no puede superar 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("La descripción no puede superar 500 caracteres.");
    }
}
