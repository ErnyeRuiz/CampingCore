using FluentValidation;

namespace CampingCore.Application.Roles.Commands.SetRolePermissions;

internal sealed class SetRolePermissionsCommandValidator : AbstractValidator<SetRolePermissionsCommand>
{
    public SetRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("El identificador del rol debe ser mayor a 0.");

        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("La lista de permisos es obligatoria.");

        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0).WithMessage("Cada identificador de permiso debe ser mayor a 0.");
    }
}
