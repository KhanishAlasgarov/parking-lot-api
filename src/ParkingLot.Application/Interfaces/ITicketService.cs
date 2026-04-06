using ParkingLot.Application.DTOs;
using ParkingLot.Domain.Entities;

namespace ParkingLot.Application.Interfaces;

public interface ITicketService
{
    Task<IssueTicketResponse> IssueTicketAsync(IssueTicketRequest request, CancellationToken ct = default);
    Task<PayTicketResponse> PayTicketAsync(PayTicketRequest request, CancellationToken ct = default);
    Task<ParkingTicket?> GetTicketAsync(string ticketNumber, CancellationToken ct = default);
    Task<ExitResponse> ExitVehicleAsync(ExitRequest request, CancellationToken ct = default);
}
