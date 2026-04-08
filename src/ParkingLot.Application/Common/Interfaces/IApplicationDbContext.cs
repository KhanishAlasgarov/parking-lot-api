using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ParkingLotEntity> ParkingLots { get; }
    DbSet<ParkingFloor> ParkingFloors { get; }
    DbSet<ParkingSpot> ParkingSpots { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<ParkingTicket> ParkingTickets { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ParkingRate> ParkingRates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
