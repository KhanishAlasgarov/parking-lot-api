namespace ParkingLot.Application.Interfaces;

public interface IDapperOccupancyRepository
{
    Task<int> GetFreeSpotCountAsync(Guid floorId);
}
