using ParkingLot.Domain.Enums;

namespace ParkingLot.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public Guid? CustomerId { get; set; }
}
