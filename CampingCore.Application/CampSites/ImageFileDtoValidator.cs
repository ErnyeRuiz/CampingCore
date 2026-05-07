using CampingCore.Domain.Entities;
using FluentValidation;

namespace CampingCore.Application.CampSites;

internal sealed class ImageFileDtoValidator : AbstractValidator<ImageFileDto>
{
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/webp",
            "image/gif",
        };

    public ImageFileDtoValidator()
    {
        RuleFor(x => x.Base64)
            .NotEmpty().WithMessage("El contenido de la imagen no puede estar vacío.")
            .MaximumLength(CampSiteImage.MaxBase64Length)
            .WithMessage("La imagen supera el tamaño máximo permitido.");

        RuleFor(x => x.ContentType)
            .Must(ct =>
            {
                if (ct is null || ct.Length == 0)
                    return true;
                var mime = ct.Split(';', 2)[0].Trim();
                return AllowedContentTypes.Contains(mime);
            })
            .WithMessage("El tipo de imagen no está permitido. Use JPEG, PNG, WebP o GIF.");
    }
}
