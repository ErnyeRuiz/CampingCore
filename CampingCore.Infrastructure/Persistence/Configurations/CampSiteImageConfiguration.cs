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

        // 2083 es la longitud máxima de una URL según el estándar RFC
        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(2083);
    }
}
