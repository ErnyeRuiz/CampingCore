using FluentValidation;

namespace CampingCore.Application.Trips.Commands.AddCampSiteToTrip;

internal sealed class AddCampSiteToTripCommandValidator : AbstractValidator<AddCampSiteToTripCommand>
{
    public AddCampSiteToTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0).WithMessage("El identificador del viaje debe ser mayor a 0.");

        RuleFor(x => x.CampSiteId)
            .GreaterThan(0).WithMessage("El identificador del sitio de camping debe ser mayor a 0.");
    }
}
