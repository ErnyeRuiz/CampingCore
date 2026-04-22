using FluentValidation;

namespace CampingCore.Application.Reviews.Commands.UpdateReview;

public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("La calificación debe estar entre 1 y 5.");
    }
}
