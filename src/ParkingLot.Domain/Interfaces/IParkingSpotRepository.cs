using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Domain.Interfaces;

public interface IParkingSpotRepository
{
    Task<ParkingSpot?> GetAvailableSpotAsync(Guid floorId, SpotType spotType, CancellationToken ct = default);
    Task<ParkingSpot?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(ParkingSpot spot, CancellationToken ct = default);
}
