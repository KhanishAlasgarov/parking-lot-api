using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class ParkingFloorConfiguration : IEntityTypeConfiguration<ParkingFloor>
{
    public void Configure(EntityTypeBuilder<ParkingFloor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.DisplayName).IsRequired();
    }
}
