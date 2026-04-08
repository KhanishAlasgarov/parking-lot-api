using ParkingLot.Domain.Enums;

namespace ParkingLot.Application.DTOs;

public class CreateFloorRequest
{
    public Guid LotId { get; set; }
    public int FloorNumber { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class CreateSpotRequest
{
    public Guid FloorId { get; set; }
    public string SpotCode { get; set; } = string.Empty;
    public SpotType SpotType { get; set; }
}

public class UpdateRateRequest
{
    public decimal RatePerHour { get; set; }
}

public class RegisterAttendantRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
