using FluentValidation;

namespace CampingCore.Application.Favorites.Commands.AddFavorite;

internal sealed class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
{
    public AddFavoriteCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El identificador del usuario debe ser mayor a 0.");

        RuleFor(x => x.CampSiteId)
            .GreaterThan(0).WithMessage("El identificador del sitio de camping debe ser mayor a 0.");
    }
}
