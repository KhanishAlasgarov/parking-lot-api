namespace ParkingLot.Application.DTOs;

public class PayTicketResponse
{
    public string ReferenceNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime PaidAt { get; set; }
}
