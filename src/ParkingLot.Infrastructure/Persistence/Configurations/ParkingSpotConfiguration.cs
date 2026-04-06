using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class ParkingSpotConfiguration : IEntityTypeConfiguration<ParkingSpot>
{
    public void Configure(EntityTypeBuilder<ParkingSpot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.SpotCode).IsRequired();

        builder.Property(x => x.SpotType)
            .HasConversion<string>();

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.HasIndex(x => new { x.FloorId, x.SpotType, x.Status });
    }
}
