using Microsoft.EntityFrameworkCore;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Interfaces;
using ParkingLot.Infrastructure.Persistence;

namespace ParkingLot.Infrastructure.Repositories;

public class ParkingTicketRepository : IParkingTicketRepository
{
    private readonly AppDbContext _context;

    public ParkingTicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ParkingTicket> CreateAsync(ParkingTicket ticket, CancellationToken ct = default)
    {
        _context.ParkingTickets.Add(ticket);
        await _context.SaveChangesAsync(ct);
        return ticket;
    }

    public async Task<ParkingTicket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken ct = default)
    {
        return await _context.ParkingTickets
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, ct);
    }

    public async Task<ParkingTicket?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ParkingTickets.FindAsync(new object[] { id }, ct);
    }

    public async Task UpdateAsync(ParkingTicket ticket, CancellationToken ct = default)
    {
        _context.ParkingTickets.Update(ticket);
        await _context.SaveChangesAsync(ct);
    }
}
