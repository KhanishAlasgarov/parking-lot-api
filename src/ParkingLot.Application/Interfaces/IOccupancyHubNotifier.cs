using ParkingLot.Application.DTOs;

namespace ParkingLot.Application.Interfaces;

public interface IOccupancyHubNotifier
{
    Task NotifyFloorUpdatedAsync(Guid floorId, FloorAvailabilityResponse availability);
}
