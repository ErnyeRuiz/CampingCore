using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class CampSiteConfiguration : IEntityTypeConfiguration<CampSite>
{
    public void Configure(EntityTypeBuilder<CampSite> builder)
    {
        builder.ToTable("CampSites");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasColumnType("NVARCHAR(MAX)");

        // DECIMAL(9,6): hasta 999.999999 grados, precisión suficiente para coordenadas GPS
        builder.Property(c => c.Latitude)
            .HasColumnName("Lattitude") // nombre con typo en el SQL original
            .HasColumnType("DECIMAL(9,6)");

        builder.Property(c => c.Longitude)
            .HasColumnType("DECIMAL(9,6)");

        // DECIMAL(10,2): hasta 99,999,999.99 por noche
        builder.Property(c => c.PricePerNight)
            .HasColumnType("DECIMAL(10,2)");

        builder.Property(c => c.HasWater)
            .HasDefaultValue(false);

        builder.Property(c => c.HasElectricity)
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.HasMany(c => c.Images)
            .WithOne(i => i.CampSite)
            .HasForeignKey(i => i.CampSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Reviews)
            .WithOne(r => r.CampSite)
            .HasForeignKey(r => r.CampSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Favorites)
            .WithOne(f => f.CampSite)
            .HasForeignKey(f => f.CampSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.TripCampSites)
            .WithOne(tc => tc.CampSite)
            .HasForeignKey(tc => tc.CampSiteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
