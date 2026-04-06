using FluentValidation;
using ParkingLot.Application.DTOs;

namespace ParkingLot.Application.Validators;

public class PayTicketRequestValidator : AbstractValidator<PayTicketRequest>
{
    public PayTicketRequestValidator()
    {
        RuleFor(v => v.TicketNumber)
            .NotEmpty().WithMessage("Ticket number is required.")
            .MaximumLength(50).WithMessage("Ticket number must not exceed 50 characters.");
            
        RuleFor(v => v.PaymentType)
            .IsInEnum().WithMessage("Invalid payment type.");
    }
}
