using FluentValidation;

namespace CampingCore.Application.Trips.Commands.UpdateTrip;

public sealed class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del viaje es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");
    }
}
