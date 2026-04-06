namespace ParkingLot.Domain.Exceptions;

public class SpotNotAvailableException : Exception
{
    public SpotNotAvailableException(string message) : base(message) { }
}
