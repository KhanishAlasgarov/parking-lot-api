namespace ParkingLot.Domain.Entities;

public class ParkingFloor
{
    public Guid Id { get; set; }
    public Guid LotId { get; set; }
    public int FloorNumber { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
