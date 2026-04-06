using ParkingLot.Domain.Enums;

namespace ParkingLot.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime PaidAt { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
}
