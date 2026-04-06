using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.LicensePlate).IsRequired();

        builder.Property(x => x.VehicleType)
            .HasConversion<string>();

        builder.HasIndex(x => x.LicensePlate).IsUnique();
    }
}
