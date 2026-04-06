using ParkingLot.Domain.Enums;

namespace ParkingLot.Domain.Entities;

public class ParkingTicket
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public Guid VehicleId { get; set; }
    public Guid SpotId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public TicketState State { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal? TotalAmount { get; set; }
}
