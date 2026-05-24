using Tazkara.Application.DTOs.Ticket;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface ITicketService
    {
        Task<ApiResponse<TicketDto>> ReserveTicketAsync(ReserveTicketRequest request, Guid userId);
        Task<ApiResponse<bool>> CancelReservationAsync(Guid ticketId, Guid userId);
        Task<ApiResponse<List<TicketDto>>> GetUserTicketsAsync(Guid userId);
        Task<ApiResponse<TicketDto>> GetTicketByIdAsync(Guid ticketId);
    }
}
