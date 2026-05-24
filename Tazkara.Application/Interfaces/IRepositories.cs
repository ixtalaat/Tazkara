using Tazkara.Domain.Entities;
using Tazkara.Application.DTOs.Event;

namespace Tazkara.Application.Interfaces
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<bool> CategoryExistsAsync(string name);
    }

    public interface IEventRepository : IAsyncRepository<Event>
    {
        Task<Event?> GetEventWithDetailsAsync(Guid id);
        Task<List<Event>> GetOrganizerEventsAsync(Guid organizerId);
        Task<(List<Event> Items, int TotalCount)> BrowseEventsAsync(EventFilterRequest filter);
    }

    public interface ITicketRepository : IAsyncRepository<Ticket>
    {
        Task<Ticket?> GetTicketWithDetailsAsync(Guid id);
        Task<List<Ticket>> GetUserTicketsAsync(Guid userId);
        Task<bool> HasUserBookedEventAsync(Guid userId, Guid eventId);
        Task<List<Ticket>> GetOrganizerTicketsAsync(Guid organizerId);
    }

    public interface IPaymentRepository : IAsyncRepository<Payment>
    {
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
    }
}
