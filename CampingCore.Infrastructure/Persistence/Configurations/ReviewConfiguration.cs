using CampingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampingCore.Infrastructure.Persistence.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        // tinyint en SQL = byte en C# (0-255). La validación de dominio ya restringe 1-5.
        builder.Property(r => r.Rating)
            .HasColumnType("tinyint");

        builder.Property(r => r.Comment)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
    }
}
