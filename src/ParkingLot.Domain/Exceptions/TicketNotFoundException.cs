namespace ParkingLot.Domain.Exceptions;

public class TicketNotFoundException : Exception
{
    public TicketNotFoundException(string message) : base(message) { }
}
