using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.CampSites.Queries.GetCampSiteById;

internal sealed class CampSiteProfile : Profile
{
    public CampSiteProfile()
    {
        CreateMap<CampSiteImage, CampSiteImageResponse>();

        CreateMap<CampSite, CampSiteResponse>()
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.Id)));
    }
}
