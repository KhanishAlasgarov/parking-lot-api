using ParkingLot.Domain.Entities;

namespace ParkingLot.Domain.Interfaces;

public interface IParkingRateRepository
{
    Task<IEnumerable<ParkingRate>> GetRatesForLotAsync(Guid lotId, CancellationToken ct = default);
}
