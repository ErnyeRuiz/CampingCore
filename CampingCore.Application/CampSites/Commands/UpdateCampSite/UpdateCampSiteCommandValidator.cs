using FluentValidation;

namespace CampingCore.Application.CampSites.Commands.UpdateCampSite;

public sealed class UpdateCampSiteCommandValidator : AbstractValidator<UpdateCampSiteCommand>
{
    public UpdateCampSiteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitud debe estar entre -90 y 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitud debe estar entre -180 y 180.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("El precio por noche debe ser mayor a 0.");
    }
}
