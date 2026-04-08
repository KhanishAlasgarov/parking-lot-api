using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingLot.Application.Interfaces;

namespace ParkingLot.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FloorsController : ControllerBase
{
    private readonly IOccupancyService _occupancyService;

    public FloorsController(IOccupancyService occupancyService)
    {
        _occupancyService = occupancyService;
    }

    [HttpGet("{floorId:guid}/availability")]
    public async Task<IActionResult> GetFloorAvailability(Guid floorId)
    {
        var response = await _occupancyService.GetFloorAvailabilityAsync(floorId);
        return Ok(response);
    }
}
