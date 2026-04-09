using Microsoft.AspNetCore.SignalR;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;

namespace ParkingLot.API.Hubs;

public class OccupancyHubNotifier : IOccupancyHubNotifier
{
    private readonly IHubContext<OccupancyHub> _hubContext;

    public OccupancyHubNotifier(IHubContext<OccupancyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyFloorUpdatedAsync(Guid floorId, FloorAvailabilityResponse availability)
    {
        await _hubContext.Clients.Group("floor-" + floorId).SendAsync("ReceiveOccupancyUpdate", availability);
    }
}
