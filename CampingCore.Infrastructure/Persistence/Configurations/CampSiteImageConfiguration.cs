using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class CampSiteImageConfiguration : IEntityTypeConfiguration<CampSiteImage>
{
    public void Configure(EntityTypeBuilder<CampSiteImage> builder)
    {
        builder.ToTable("CampSiteImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(2083);
    }
}
