using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class TripCampSiteConfiguration : IEntityTypeConfiguration<TripCampSite>
{
    public void Configure(EntityTypeBuilder<TripCampSite> builder)
    {
        builder.ToTable("TripCampSites");

        builder.HasKey(tc => tc.Id);
    }
}
