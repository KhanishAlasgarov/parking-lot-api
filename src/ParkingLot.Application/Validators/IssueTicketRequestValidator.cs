using FluentValidation;
using ParkingLot.Application.DTOs;

namespace ParkingLot.Application.Validators;

public class IssueTicketRequestValidator : AbstractValidator<IssueTicketRequest>
{
    public IssueTicketRequestValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .MaximumLength(20).WithMessage("License plate must not exceed 20 characters.");
            
        RuleFor(v => v.VehicleType)
            .IsInEnum().WithMessage("Invalid vehicle type.");
            
        RuleFor(v => v.SpotType)
            .IsInEnum().WithMessage("Invalid spot type.");
            
        RuleFor(v => v.FloorId)
            .NotEmpty().WithMessage("Floor ID is required.");
    }
}
