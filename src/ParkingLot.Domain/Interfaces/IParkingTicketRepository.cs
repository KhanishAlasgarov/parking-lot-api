using ParkingLot.Domain.Entities;

namespace ParkingLot.Domain.Interfaces;

public interface IParkingTicketRepository
{
    Task<ParkingTicket> CreateAsync(ParkingTicket ticket, CancellationToken ct = default);
    Task<ParkingTicket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken ct = default);
    Task<ParkingTicket?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(ParkingTicket ticket, CancellationToken ct = default);
}
