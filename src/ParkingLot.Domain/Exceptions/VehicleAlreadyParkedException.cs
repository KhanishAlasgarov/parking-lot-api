namespace ParkingLot.Domain.Exceptions;

public class VehicleAlreadyParkedException : Exception
{
    public VehicleAlreadyParkedException(string message) : base(message) { }
}
