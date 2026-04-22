using FluentValidation;

namespace CampingCore.Application.Favorites.Commands.RemoveFavorite;

internal sealed class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
{
    public RemoveFavoriteCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El identificador del usuario debe ser mayor a 0.");

        RuleFor(x => x.CampSiteId)
            .GreaterThan(0).WithMessage("El identificador del sitio de camping debe ser mayor a 0.");
    }
}
