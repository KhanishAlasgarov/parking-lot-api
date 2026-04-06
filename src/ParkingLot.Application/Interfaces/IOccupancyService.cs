using ParkingLot.Application.DTOs;

namespace ParkingLot.Application.Interfaces;

public interface IOccupancyService
{
    Task<FloorAvailabilityResponse> GetFloorAvailabilityAsync(Guid floorId, CancellationToken ct = default);
}
