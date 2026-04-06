using ParkingLot.Domain.Entities;

namespace ParkingLot.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByLicensePlateAsync(string plate, CancellationToken ct = default);
    Task<Vehicle> CreateAsync(Vehicle vehicle, CancellationToken ct = default);
}
