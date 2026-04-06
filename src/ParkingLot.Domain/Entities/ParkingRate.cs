namespace ParkingLot.Domain.Entities;

public class ParkingRate
{
    public Guid Id { get; set; }
    public Guid LotId { get; set; }
    public int HourFrom { get; set; }
    public int HourTo { get; set; }
    public decimal RatePerHour { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
