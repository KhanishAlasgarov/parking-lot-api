using Microsoft.EntityFrameworkCore;
using ParkingLot.Application.Common.Interfaces;
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
    private readonly IApplicationDbContext _dbContext;
    private readonly IOccupancyCache _cache;
    private readonly IOccupancyHubNotifier _notifier;
    private readonly IOccupancyService _occupancyService;

    public TicketService(
        IVehicleRepository vehicleRepo,
        IParkingSpotRepository spotRepo,
        IParkingTicketRepository ticketRepo,
        IPricingEngine pricingEngine,
        IApplicationDbContext dbContext,
        IOccupancyCache cache,
        IOccupancyHubNotifier notifier,
        IOccupancyService occupancyService)
    {
        _vehicleRepo = vehicleRepo;
        _spotRepo = spotRepo;
        _ticketRepo = ticketRepo;
        _pricingEngine = pricingEngine;
        _dbContext = dbContext;
        _cache = cache;
        _notifier = notifier;
        _occupancyService = occupancyService;
    }

    public async Task<IssueTicketResponse> IssueTicketAsync(IssueTicketRequest request, CancellationToken ct = default)
    {
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

        var existingTickets = _dbContext.ParkingTickets
            .Where(t => t.VehicleId == vehicle.Id &&
                        (t.State == TicketState.Active || t.State == TicketState.Issued));

        if (await existingTickets.AnyAsync(ct))
        {
            throw new VehicleAlreadyParkedException(
                $"Vehicle {request.LicensePlate} already has an active parking ticket.");
        }

        ParkingTicket ticket = null!;
        ParkingSpot spot = null!;

        int maxRetries = 1;
        for (int i = 0; i <= maxRetries; i++)
        {
            try
            {
                spot = await _spotRepo.GetAvailableSpotAsync(request.FloorId, request.SpotType, ct);
                if (spot is null)
                {
                    throw new SpotNotAvailableException(
                        $"No available {request.SpotType} spot on the requested floor.");
                }

                ticket = new ParkingTicket
                {
                    Id = Guid.NewGuid(),
                    TicketNumber = "TKT-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    VehicleId = vehicle.Id,
                    SpotId = spot.Id,
                    State = TicketState.Issued,
                    EntryTime = DateTime.UtcNow
                };
                await _ticketRepo.CreateAsync(ticket, ct);

                spot.Status = SpotStatus.Occupied;
                await _spotRepo.UpdateAsync(spot, ct);

                await _dbContext.SaveChangesAsync(ct);
                break;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (i == maxRetries)
                {
                    throw new SpotNotAvailableException("Spot was taken during reservation. Please try again.");
                }
            }
        }

        // Redis cache invalidation and SignalR notification
        await _cache.InvalidateAsync(request.FloorId);
        var floorStatus = await _occupancyService.GetFloorAvailabilityAsync(request.FloorId, ct);
        await _notifier.NotifyFloorUpdatedAsync(request.FloorId, floorStatus);

        return new IssueTicketResponse
        {
            TicketNumber = ticket.TicketNumber,
            SpotCode = spot.SpotCode,
            EntryTime = ticket.EntryTime
        };
    }

    public async Task<PayTicketResponse> PayTicketAsync(PayTicketRequest request, CancellationToken ct = default)
    {
        var ticket = await _ticketRepo.GetByTicketNumberAsync(request.TicketNumber, ct);
        if (ticket is null)
        {
            throw new TicketNotFoundException($"Ticket {request.TicketNumber} not found.");
        }

        if (ticket.State != TicketState.Active && ticket.State != TicketState.Issued)
        {
            throw new InvalidTicketStateException(
                $"Ticket {request.TicketNumber} is in state {ticket.State} and cannot be paid.");
        }

        var exitTime = DateTime.UtcNow;
        var spot = await _spotRepo.GetByIdAsync(ticket.SpotId, ct);
        var lotId = spot is not null
            ? (await _dbContext.ParkingFloors.FirstOrDefaultAsync(f => f.Id == spot.FloorId, ct))?.LotId ?? Guid.Empty
            : Guid.Empty;

        var totalAmount = await _pricingEngine.CalculateFeeAsync(lotId, ticket.EntryTime, exitTime, ct);

        ticket.State = TicketState.Paid;
        ticket.PaidAt = DateTime.UtcNow;
        ticket.TotalAmount = totalAmount;
        await _ticketRepo.UpdateAsync(ticket, ct);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            Amount = totalAmount,
            PaymentType = request.PaymentType,
            PaidAt = DateTime.UtcNow,
            ReferenceNo = "PAY-" + Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
        _dbContext.Payments.Add(payment);

        await _dbContext.SaveChangesAsync(ct);

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
        var ticket = await _ticketRepo.GetByTicketNumberAsync(request.TicketNumber, ct);
        if (ticket is null)
        {
            throw new TicketNotFoundException($"Ticket {request.TicketNumber} not found.");
        }

        if (ticket.State != TicketState.Paid)
        {
            throw new InvalidTicketStateException("Payment required before exit.");
        }

        ticket.State = TicketState.Closed;
        ticket.ExitTime = DateTime.UtcNow;
        await _ticketRepo.UpdateAsync(ticket, ct);

        var spot = await _spotRepo.GetByIdAsync(ticket.SpotId, ct);
        Guid? floorId = null;

        if (spot is not null)
        {
            spot.Status = SpotStatus.Free;
            await _spotRepo.UpdateAsync(spot, ct);
            floorId = spot.FloorId;
        }

        await _dbContext.SaveChangesAsync(ct);

        if (floorId.HasValue)
        {
            // Redis cache invalidation and SignalR notification
            await _cache.InvalidateAsync(floorId.Value);
            var floorStatus = await _occupancyService.GetFloorAvailabilityAsync(floorId.Value, ct);
            await _notifier.NotifyFloorUpdatedAsync(floorId.Value, floorStatus);
        }

        var vehicle = await _dbContext.Vehicles
            .FirstOrDefaultAsync(v => v.Id == ticket.VehicleId, ct);

        return new ExitResponse
        {
            LicensePlate = vehicle?.LicensePlate ?? string.Empty,
            TotalAmount = ticket.TotalAmount ?? 0m,
            ExitTime = ticket.ExitTime ?? DateTime.UtcNow
        };
    }
}
