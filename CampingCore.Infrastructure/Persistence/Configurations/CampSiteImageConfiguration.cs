using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class CampSiteImageConfiguration : IEntityTypeConfiguration<CampSiteImage>
{
    public void Configure(EntityTypeBuilder<CampSiteImage> builder)
    {
        builder.ToTable("CampSiteImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageBase64)
            .IsRequired()
            .HasColumnType("nvarchar(MAX)");
    }
}
