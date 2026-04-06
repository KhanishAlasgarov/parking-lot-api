using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Infrastructure.Persistence.Configurations;

public class ParkingTicketConfiguration : IEntityTypeConfiguration<ParkingTicket>
{
    public void Configure(EntityTypeBuilder<ParkingTicket> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.TicketNumber).IsRequired();

        builder.Property(x => x.State)
            .HasConversion<string>();

        builder.HasIndex(x => x.TicketNumber).IsUnique();
        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.State);
    }
}
