namespace ParkingLot.Application.DTOs;

public class FloorAvailabilityResponse
{
    public Guid FloorId { get; set; }
    public int FreeSpotsCount { get; set; }
    public int TotalSpots { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
}
