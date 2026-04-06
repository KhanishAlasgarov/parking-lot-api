namespace ParkingLot.Application.DTOs;

public class ExitResponse
{
    public string LicensePlate { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime ExitTime { get; set; }
}
