using Microsoft.AspNetCore.Identity;
using ParkingLot.Domain.Enums;

namespace ParkingLot.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public UserRole Role { get; set; }
}
