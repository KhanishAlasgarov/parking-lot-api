using ParkingLot.Application.DTOs;

namespace ParkingLot.Application.Interfaces;

public interface IOccupancyCache
{
    Task<FloorAvailabilityResponse?> GetAsync(Guid floorId);
    Task SetAsync(Guid floorId, FloorAvailabilityResponse data, TimeSpan ttl);
    Task InvalidateAsync(Guid floorId);
}
