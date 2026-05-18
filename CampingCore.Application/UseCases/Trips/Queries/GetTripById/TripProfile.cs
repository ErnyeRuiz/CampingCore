using AutoMapper;
using CampingCore.Application.UseCases.Shared;
using CampingCore.Application.UseCases.Trips.Queries.GetTripById;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Trips.Queries.GetTripById;

internal sealed class TripProfile : Profile
{
    public TripProfile()
    {
        CreateMap<Trip, TripResponse>()
            .ConstructUsing((src, ctx) => new TripResponse(
                src.Id,
                src.UserId,
                src.Name,
                src.StartDate,
                src.EndDate,
                src.CampSites.OrderBy(tc => tc.Id)
                    .Select(tc => ctx.Mapper.Map<CampSiteSummary>(tc.CampSite))
                    .ToList()));

        CreateMap<CampSite, CampSiteSummary>()
            .ConstructUsing((src, _) => new CampSiteSummary(
                src.Id,
                src.Name,
                src.Description,
                src.PricePerNight,
                src.IdProvincia,
                src.IdCanton,
                src.IdDistrito,
                src.Images.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault()));
    }
}
