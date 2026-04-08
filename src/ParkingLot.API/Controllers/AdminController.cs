using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingLot.Application.Common.Interfaces;
using ParkingLot.Application.DTOs;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;
using ParkingLot.Infrastructure.Identity;

namespace ParkingLot.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public AdminController(IApplicationDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpPost("floors")]
    public async Task<IActionResult> CreateFloor([FromBody] CreateFloorRequest request)
    {
        var floor = new ParkingFloor
        {
            Id = Guid.NewGuid(),
            LotId = request.LotId,
            FloorNumber = request.FloorNumber,
            DisplayName = request.DisplayName
        };

        _dbContext.ParkingFloors.Add(floor);
        await _dbContext.SaveChangesAsync();

        return StatusCode(201, floor);
    }

    [HttpPost("spots")]
    public async Task<IActionResult> CreateSpot([FromBody] CreateSpotRequest request)
    {
        var spot = new ParkingSpot
        {
            Id = Guid.NewGuid(),
            FloorId = request.FloorId,
            SpotCode = request.SpotCode,
            SpotType = request.SpotType,
            Status = SpotStatus.Free
        };

        _dbContext.ParkingSpots.Add(spot);
        await _dbContext.SaveChangesAsync();

        return StatusCode(201, spot);
    }

    [HttpPut("rates/{rateId:guid}")]
    public async Task<IActionResult> UpdateRate(Guid rateId, [FromBody] UpdateRateRequest request)
    {
        var rate = await _dbContext.ParkingRates.FindAsync(rateId);
        if (rate == null)
            return NotFound("Rate not found.");

        rate.RatePerHour = request.RatePerHour;
        await _dbContext.SaveChangesAsync();

        return Ok(rate);
    }

    [HttpPost("attendants")]
    public async Task<IActionResult> RegisterAttendant([FromBody] RegisterAttendantRequest request)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            Role = UserRole.Attendant,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return StatusCode(201, new { Message = "Attendant registered successfully." });
    }

    [HttpDelete("attendants/{id:guid}")]
    public async Task<IActionResult> SoftDeleteAttendant(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound("Attendant not found.");

        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return Ok(new { Message = "Attendant deactivated." });
    }
}
