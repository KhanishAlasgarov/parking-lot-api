using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Interfaces;
using ParkingLot.Infrastructure.Persistence;

namespace ParkingLot.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string plate, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.LicensePlate == plate, ct);
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync(ct);
        return vehicle;
    }
}
