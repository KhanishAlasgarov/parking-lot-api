using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Interfaces;
using ParkingLot.Infrastructure.Persistence;

namespace ParkingLot.Infrastructure.Repositories;

public class ParkingRateRepository : IParkingRateRepository
{
    private readonly AppDbContext _context;

    public ParkingRateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ParkingRate>> GetRatesForLotAsync(Guid lotId, CancellationToken ct = default)
    {
        return await _context.ParkingRates
            .Where(r => r.LotId == lotId)
            .ToListAsync(ct);
    }
}
