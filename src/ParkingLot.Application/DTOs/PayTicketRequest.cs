using ParkingLot.Domain.Enums;

namespace ParkingLot.Application.DTOs;

public class PayTicketRequest
{
    public string TicketNumber { get; set; } = string.Empty;
    public PaymentType PaymentType { get; set; }
}
