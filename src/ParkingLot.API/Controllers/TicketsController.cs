using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;

namespace ParkingLot.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<IActionResult> IssueTicket([FromBody] IssueTicketRequest request)
    {
        // Model validation is automatically handled by the [ApiController] attribute,
        // but since we are using FluentValidation, we should ensure the pipeline catches errors.
        // If not using automatic validation filter, we would validate manually.
        // Assuming automatic validation filter is set up or we can just proceed:
        
        var response = await _ticketService.IssueTicketAsync(request);
        return CreatedAtAction(nameof(GetTicket), new { number = response.TicketNumber }, response);
    }

    [HttpGet("{number}")]
    public async Task<IActionResult> GetTicket(string number)
    {
        var ticket = await _ticketService.GetTicketAsync(number);
        if (ticket == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = $"Ticket {number} not found."
            });
        }

        return Ok(ticket);
    }

    [HttpPost("{number}/pay")]
    public async Task<IActionResult> PayTicket(string number, [FromBody] PayTicketRequest request)
    {
        if (number != request.TicketNumber)
        {
            return BadRequest("Ticket number in URL and body must match.");
        }

        var response = await _ticketService.PayTicketAsync(request);
        return Ok(response);
    }

    [HttpPost("{number}/exit")]
    public async Task<IActionResult> ExitVehicle(string number)
    {
        var response = await _ticketService.ExitVehicleAsync(new ExitRequest { TicketNumber = number });
        return Ok(response);
    }
}
