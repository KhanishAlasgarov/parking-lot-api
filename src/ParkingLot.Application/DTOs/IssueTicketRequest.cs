using ParkingLot.Domain.Enums;

namespace ParkingLot.Application.DTOs;

public class IssueTicketRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public SpotType SpotType { get; set; }
    public Guid FloorId { get; set; }
}
