using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class ParkingLotEntityConfiguration : IEntityTypeConfiguration<ParkingLotEntity>
{
    public void Configure(EntityTypeBuilder<ParkingLotEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Address).IsRequired();
    }
}
