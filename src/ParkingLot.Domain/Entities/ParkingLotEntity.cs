namespace ParkingLot.Domain.Entities;

public class ParkingLotEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
}
