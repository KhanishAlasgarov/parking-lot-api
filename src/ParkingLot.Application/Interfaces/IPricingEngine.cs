namespace ParkingLot.Application.Interfaces;

public interface IPricingEngine
{
    Task<decimal> CalculateFeeAsync(Guid lotId, DateTime entryTime, DateTime exitTime, CancellationToken ct = default);
}
