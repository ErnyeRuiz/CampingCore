using AutoMapper;
using CampingCore.Domain.Entities;

namespace CampingCore.Application.Trips.Queries.GetTripById;

internal sealed class TripProfile : Profile
{
    public TripProfile()
    {
        CreateMap<Trip, TripResponse>();
    }
}
