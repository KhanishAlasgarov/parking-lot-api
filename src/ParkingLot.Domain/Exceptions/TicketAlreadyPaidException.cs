namespace ParkingLot.Domain.Exceptions;

public class TicketAlreadyPaidException : Exception
{
    public TicketAlreadyPaidException(string message) : base(message) { }
}
