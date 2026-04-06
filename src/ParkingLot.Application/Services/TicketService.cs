using Microsoft.EntityFrameworkCore;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Enums;
using ParkingLot.Domain.Exceptions;
using ParkingLot.Domain.Interfaces;

namespace ParkingLot.Application.Services;

public class TicketService : ITicketService
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IParkingSpotRepository _spotRepo;
    private readonly IParkingTicketRepository _ticketRepo;
    private readonly IPricingEngine _pricingEngine;
    private readonly DbContext _dbContext;

    public TicketService(
        IVehicleRepository vehicleRepo,
        IParkingSpotRepository spotRepo,
        IParkingTicketRepository ticketRepo,
        IPricingEngine pricingEngine,
        DbContext dbContext)
    {
        _vehicleRepo = vehicleRepo;
        _spotRepo = spotRepo;
        _ticketRepo = ticketRepo;
        _pricingEngine = pricingEngine;
        _dbContext = dbContext;
    }

    public async Task<IssueTicketResponse> IssueTicketAsync(IssueTicketRequest request, CancellationToken ct = default)
    {
        // 1. Check vehicle by license plate; create if not found
        var vehicle = await _vehicleRepo.GetByLicensePlateAsync(request.LicensePlate, ct);
        if (vehicle is null)
        {
            vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                LicensePlate = request.LicensePlate,
                VehicleType = request.VehicleType
            };
            await _vehicleRepo.CreateAsync(vehicle, ct);
        }

        // 2. Check if vehicle already has an active ticket
        var existingTickets = _dbContext.Set<ParkingTicket>()
            .Where(t => t.VehicleId == vehicle.Id &&
                        (t.State == TicketState.Active || t.State == TicketState.Issued));

        if (await existingTickets.AnyAsync(ct))
        {
            throw new VehicleAlreadyParkedException(
                $"Vehicle {request.LicensePlate} already has an active parking ticket.");
        }

        // 3. Find available spot
        var spot = await _spotRepo.GetAvailableSpotAsync(request.FloorId, request.SpotType, ct);
        if (spot is null)
        {
            throw new SpotNotAvailableException(
                $"No available {request.SpotType} spot on the requested floor.");
        }

        // 4. Create parking ticket
        var ticket = new ParkingTicket
        {
            Id = Guid.NewGuid(),
            TicketNumber = "TKT-" + DateTime.UtcNow.Ticks,
            VehicleId = vehicle.Id,
            SpotId = spot.Id,
            State = TicketState.Issued,
            EntryTime = DateTime.UtcNow
        };
        await _ticketRepo.CreateAsync(ticket, ct);

        // 5. Update spot status to Occupied
        spot.Status = SpotStatus.Occupied;
        await _spotRepo.UpdateAsync(spot, ct);

        // 6. Save all changes
        await _dbContext.SaveChangesAsync(ct);

        // 7. Return response
        return new IssueTicketResponse
        {
            TicketNumber = ticket.TicketNumber,
            SpotCode = spot.SpotCode,
            EntryTime = ticket.EntryTime
        };
    }

    public async Task<PayTicketResponse> PayTicketAsync(PayTicketRequest request, CancellationToken ct = default)
    {
        // 1. Get ticket by TicketNumber
        var ticket = await _ticketRepo.GetByTicketNumberAsync(request.TicketNumber, ct);
        if (ticket is null)
        {
            throw new TicketNotFoundException($"Ticket {request.TicketNumber} not found.");
        }

        // 2. Validate state
        if (ticket.State != TicketState.Active && ticket.State != TicketState.Issued)
        {
            throw new InvalidTicketStateException(
                $"Ticket {request.TicketNumber} is in state {ticket.State} and cannot be paid.");
        }

        // 3. Calculate fee
        var exitTime = DateTime.UtcNow;
        var spot = await _spotRepo.GetByIdAsync(ticket.SpotId, ct);
        var lotId = spot is not null
            ? (await _dbContext.Set<ParkingFloor>().FirstOrDefaultAsync(f => f.Id == spot.FloorId, ct))?.LotId ?? Guid.Empty
            : Guid.Empty;

        var totalAmount = await _pricingEngine.CalculateFeeAsync(lotId, ticket.EntryTime, exitTime, ct);

        // 4. Update ticket
        ticket.State = TicketState.Paid;
        ticket.PaidAt = DateTime.UtcNow;
        ticket.TotalAmount = totalAmount;
        await _ticketRepo.UpdateAsync(ticket, ct);

        // 5. Create payment
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            Amount = totalAmount,
            PaymentType = request.PaymentType,
            PaidAt = DateTime.UtcNow,
            ReferenceNo = "PAY-" + DateTime.UtcNow.Ticks
        };
        _dbContext.Set<Payment>().Add(payment);

        // 6. Save changes
        await _dbContext.SaveChangesAsync(ct);

        // 7. Return response
        return new PayTicketResponse
        {
            ReferenceNo = payment.ReferenceNo,
            TotalAmount = totalAmount,
            PaidAt = payment.PaidAt
        };
    }

    public async Task<ParkingTicket?> GetTicketAsync(string ticketNumber, CancellationToken ct = default)
    {
        return await _ticketRepo.GetByTicketNumberAsync(ticketNumber, ct);
    }

    public async Task<ExitResponse> ExitVehicleAsync(ExitRequest request, CancellationToken ct = default)
    {
        // 1. Get ticket and validate state
        var ticket = await _ticketRepo.GetByTicketNumberAsync(request.TicketNumber, ct);
        if (ticket is null)
        {
            throw new TicketNotFoundException($"Ticket {request.TicketNumber} not found.");
        }

        if (ticket.State != TicketState.Paid)
        {
            throw new InvalidTicketStateException("Payment required before exit.");
        }

        // 2. Close ticket
        ticket.State = TicketState.Closed;
        ticket.ExitTime = DateTime.UtcNow;
        await _ticketRepo.UpdateAsync(ticket, ct);

        // 3. Free the spot
        var spot = await _spotRepo.GetByIdAsync(ticket.SpotId, ct);
        if (spot is not null)
        {
            spot.Status = SpotStatus.Free;
            await _spotRepo.UpdateAsync(spot, ct);
        }

        // 4. Save changes
        await _dbContext.SaveChangesAsync(ct);

        // 5. Get vehicle info for response
        var vehicle = await _dbContext.Set<Vehicle>()
            .FirstOrDefaultAsync(v => v.Id == ticket.VehicleId, ct);

        return new ExitResponse
        {
            LicensePlate = vehicle?.LicensePlate ?? string.Empty,
            TotalAmount = ticket.TotalAmount ?? 0m,
            ExitTime = ticket.ExitTime ?? DateTime.UtcNow
        };
    }
}
