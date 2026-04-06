namespace ParkingLot.Application.DTOs;

public class IssueTicketResponse
{
    public string TicketNumber { get; set; } = string.Empty;
    public string SpotCode { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
}
