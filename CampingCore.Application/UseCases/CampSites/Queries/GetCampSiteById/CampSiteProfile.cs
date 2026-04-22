using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.CampSites.Queries.GetCampSiteById;

internal sealed class CampSiteProfile : Profile
{
    public CampSiteProfile()
    {
        CreateMap<CampSite, CampSiteResponse>();
    }
}
