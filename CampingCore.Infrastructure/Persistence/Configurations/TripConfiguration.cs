using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // "date" en SQL almacena solo fecha sin hora, compatible con DateOnly en .NET 6+
        builder.Property(t => t.StartDate)
            .HasColumnType("date");

        builder.Property(t => t.EndDate)
            .HasColumnType("date");

        builder.HasMany(t => t.CampSites)
            .WithOne(tc => tc.Trip)
            .HasForeignKey(tc => tc.TripId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
