namespace ParkingLot.Domain.Enums;

public enum TicketState
{
    Issued,
    Active,
    PaymentPending,
    Paid,
    Closed,
    Expired,
    Void
}
