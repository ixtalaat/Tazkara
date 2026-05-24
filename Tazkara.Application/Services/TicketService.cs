using Mapster;
using Tazkara.Application.DTOs.Ticket;
using Tazkara.Application.Exceptions;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;

namespace Tazkara.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
        }

        public async Task<ApiResponse<TicketDto>> ReserveTicketAsync(ReserveTicketRequest request, Guid userId)
        {
            var targetEvent = await _eventRepository.GetByIdAsync(request.EventId);
            if (targetEvent == null)
                return ApiResponse<TicketDto>.ErrorResponse("Event not found.");

            if (targetEvent.Status != EventStatus.Published)
                return ApiResponse<TicketDto>.ErrorResponse("Event is not open for reservations.");

            if (targetEvent.StartDate < DateTime.UtcNow)
                return ApiResponse<TicketDto>.ErrorResponse("Cannot book a ticket for a past event.");

            var hasBooked = await _ticketRepository.HasUserBookedEventAsync(userId, request.EventId);
            if (hasBooked)
                return ApiResponse<TicketDto>.ErrorResponse("You have already booked a ticket for this event.");

            if (targetEvent.AvailableTickets <= 0)
                return ApiResponse<TicketDto>.ErrorResponse("This event is sold out.");

            targetEvent.AvailableTickets -= 1;
            
            try
            {
                await _eventRepository.UpdateAsync(targetEvent);
            }
            catch (ConcurrencyException)
            {
                return ApiResponse<TicketDto>.ErrorResponse("Concurrency conflict: The event tickets were updated by another user. Please try again.");
            }

            var ticket = new Ticket
            {
                TicketNumber = $"TZK-{DateTime.UtcNow.Ticks}-{Guid.NewGuid().ToString().Substring(0, 4)}".ToUpper(),
                EventId = request.EventId,
                UserId = userId,
                Status = TicketStatus.Reserved,
                PaymentStatus = PaymentStatus.Pending
            };

            await _ticketRepository.AddAsync(ticket);

            return await GetTicketByIdAsync(ticket.Id);
        }

        public async Task<ApiResponse<bool>> CancelReservationAsync(Guid ticketId, Guid userId)
        {
            var ticket = await _ticketRepository.GetTicketWithDetailsAsync(ticketId);
            if (ticket == null || ticket.UserId != userId)
                return ApiResponse<bool>.ErrorResponse("Ticket not found.");

            if (ticket.Status == TicketStatus.Cancelled)
                return ApiResponse<bool>.ErrorResponse("Ticket is already cancelled.");

            ticket.Status = TicketStatus.Cancelled;
            await _ticketRepository.UpdateAsync(ticket);

            var targetEvent = await _eventRepository.GetByIdAsync(ticket.EventId);
            if (targetEvent != null)
            {
                targetEvent.AvailableTickets += 1;
                await _eventRepository.UpdateAsync(targetEvent);
            }

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<List<TicketDto>>> GetUserTicketsAsync(Guid userId)
        {
            var tickets = await _ticketRepository.GetUserTicketsAsync(userId);
            return ApiResponse<List<TicketDto>>.SuccessResponse(tickets.Adapt<List<TicketDto>>());
        }

        public async Task<ApiResponse<TicketDto>> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetTicketWithDetailsAsync(ticketId);
            if (ticket == null)
                return ApiResponse<TicketDto>.ErrorResponse("Ticket not found.");

            return ApiResponse<TicketDto>.SuccessResponse(ticket.Adapt<TicketDto>());
        }
    }
}
