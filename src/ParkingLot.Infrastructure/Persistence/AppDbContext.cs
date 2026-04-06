using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ParkingLotEntity> ParkingLots => Set<ParkingLotEntity>();
    public DbSet<ParkingFloor> ParkingFloors => Set<ParkingFloor>();
    public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ParkingTicket> ParkingTickets => Set<ParkingTicket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ParkingRate> ParkingRates => Set<ParkingRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
