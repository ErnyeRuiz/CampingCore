using FluentValidation;

namespace CampingCore.Application.Reviews.Commands.CreateReview;

internal sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El identificador del usuario debe ser mayor a 0.");

        RuleFor(x => x.CampSiteId)
            .GreaterThan(0).WithMessage("El identificador del sitio de camping debe ser mayor a 0.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5).WithMessage("La calificación debe estar entre 1 y 5.");
    }
}
