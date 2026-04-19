using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;
using ParkingLot.Domain.Enums;
using ParkingLot.Infrastructure.Identity;

namespace ParkingLot.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public AuthController(UserManager<AppUser> userManager, IJwtService jwtService, IConfiguration config)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest("User with this email already exists.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            Role = UserRole.Customer
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(201, new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.Role.ToString());
        
        var expiryMinutesString = _config["JwtSettings:ExpiryMinutes"] ?? "60";
        var expiryMinutes = int.Parse(expiryMinutesString);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        return Ok(new
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }
}
