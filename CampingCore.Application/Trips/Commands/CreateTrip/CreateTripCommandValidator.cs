using FluentValidation;

namespace CampingCore.Application.Trips.Commands.CreateTrip;

internal sealed class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El identificador del usuario debe ser mayor a 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del viaje es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del viaje no puede superar 100 caracteres.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");
    }
}
