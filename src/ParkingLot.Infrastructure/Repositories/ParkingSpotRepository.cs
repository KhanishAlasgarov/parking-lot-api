using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;
using ParkingLot.Domain.Interfaces;
using ParkingLot.Infrastructure.Persistence;

namespace ParkingLot.Infrastructure.Repositories;

public class ParkingSpotRepository : IParkingSpotRepository
{
    private readonly AppDbContext _context;

    public ParkingSpotRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ParkingSpot?> GetAvailableSpotAsync(Guid floorId, SpotType spotType, CancellationToken ct = default)
    {
        return await _context.ParkingSpots
            .Where(s => s.FloorId == floorId && s.SpotType == spotType && s.Status == SpotStatus.Free)
            .OrderBy(s => s.SpotCode)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ParkingSpot?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ParkingSpots.FindAsync(new object[] { id }, ct);
    }

    public async Task UpdateAsync(ParkingSpot spot, CancellationToken ct = default)
    {
        _context.ParkingSpots.Update(spot);
        await _context.SaveChangesAsync(ct);
    }
}
