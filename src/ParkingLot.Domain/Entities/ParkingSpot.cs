using ParkingLot.Domain.Enums;

namespace ParkingLot.Domain.Entities;

public class ParkingSpot
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public string SpotCode { get; set; } = string.Empty;
    public SpotType SpotType { get; set; }
    public SpotStatus Status { get; set; }
    public bool HasEvCharger { get; set; }
}
