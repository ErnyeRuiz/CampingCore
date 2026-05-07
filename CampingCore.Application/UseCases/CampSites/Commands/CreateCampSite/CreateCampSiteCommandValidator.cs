using CampingCore.Application.CampSites;
using CampingCore.Domain.Entities;
using FluentValidation;

namespace CampingCore.Application.CampSites.Commands.CreateCampSite;

internal sealed class CreateCampSiteCommandValidator : AbstractValidator<CreateCampSiteCommand>
{
    public CreateCampSiteCommandValidator()
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

        RuleFor(x => x.CreatedByUserId)
            .GreaterThan(0).WithMessage("El identificador del usuario debe ser mayor a 0.");

        RuleFor(x => x.IdProvincia)
            .GreaterThan(0).WithMessage("El identificador de provincia debe ser mayor a 0.");

        RuleFor(x => x.IdCanton)
            .GreaterThan(0).WithMessage("El identificador de cantón debe ser mayor a 0.");

        RuleFor(x => x.IdDistrito)
            .GreaterThan(0).WithMessage("El identificador de distrito debe ser mayor a 0.");

        RuleFor(x => x.DireccionExacta)
            .MaximumLength(500).WithMessage("La dirección exacta no puede superar 500 caracteres.")
            .When(x => x.DireccionExacta is not null);

        RuleFor(x => x.NewImages)
            .Must(l => l.Count <= CampSite.MaxImagesPerCampSite)
            .WithMessage($"No puede adjuntar más de {CampSite.MaxImagesPerCampSite} imágenes.");

        RuleForEach(x => x.NewImages)
            .SetValidator(new ImageFileDtoValidator());
    }
}
