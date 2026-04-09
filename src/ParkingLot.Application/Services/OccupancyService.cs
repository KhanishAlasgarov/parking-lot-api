using Microsoft.EntityFrameworkCore;
using ParkingLot.Application.Common.Interfaces;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Application.Services;

public class OccupancyService : IOccupancyService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IOccupancyCache _cache;

    public OccupancyService(IApplicationDbContext dbContext, IOccupancyCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<FloorAvailabilityResponse> GetFloorAvailabilityAsync(Guid floorId, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync(floorId);
        if (cached != null)
        {
            return cached;
        }

        var spots = await _dbContext.ParkingSpots
            .Where(s => s.FloorId == floorId)
            .ToListAsync(ct);

        var freeSpots = spots.Where(s => s.Status == SpotStatus.Free).ToList();

        var byType = freeSpots
            .GroupBy(s => s.SpotType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var response = new FloorAvailabilityResponse
        {
            FloorId = floorId,
            FreeSpotsCount = freeSpots.Count,
            TotalSpots = spots.Count,
            ByType = byType
        };

        await _cache.SetAsync(floorId, response, TimeSpan.FromSeconds(5));
        return response;
    }
}
