using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class ParkingRateConfiguration : IEntityTypeConfiguration<ParkingRate>
{
    public void Configure(EntityTypeBuilder<ParkingRate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RatePerHour)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new { x.LotId, x.HourFrom, x.EffectiveFrom });
    }
}
