using FluentValidation;

namespace CampingCore.Application.Dashboard.Queries.GetCampSiteStats;

internal sealed class GetCampSiteStatsQueryValidator : AbstractValidator<GetCampSiteStatsQuery>
{
    public GetCampSiteStatsQueryValidator()
    {
        RuleFor(x => x.IdProvincia)
            .Must(v => !v.HasValue || v.Value > 0)
            .WithMessage("idProvincia debe ser mayor que 0 si se envía.");

        RuleFor(x => x.IdCanton)
            .Must(v => !v.HasValue || v.Value > 0)
            .WithMessage("idCanton debe ser mayor que 0 si se envía.");

        RuleFor(x => x.IdDistrito)
            .Must(v => !v.HasValue || v.Value > 0)
            .WithMessage("idDistrito debe ser mayor que 0 si se envía.");
    }
}
