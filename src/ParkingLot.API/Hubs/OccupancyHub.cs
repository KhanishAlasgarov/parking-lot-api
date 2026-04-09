using Microsoft.AspNetCore.SignalR;

namespace ParkingLot.API.Hubs;

public class OccupancyHub : Hub
{
    public async Task JoinFloorAsync(Guid floorId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "floor-" + floorId);
    }

    public async Task LeaveFloorAsync(Guid floorId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "floor-" + floorId);
    }
}
