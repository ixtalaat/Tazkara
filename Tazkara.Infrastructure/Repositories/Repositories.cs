using Microsoft.EntityFrameworkCore;
using Tazkara.Application.DTOs.Event;
using Tazkara.Application.Interfaces;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;
using Tazkara.Infrastructure.Data;

namespace Tazkara.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _dbContext.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }

    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Event?> GetEventWithDetailsAsync(Guid id)
        {
            return await _dbContext.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetOrganizerEventsAsync(Guid organizerId)
        {
            return await _dbContext.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<Event> Items, int TotalCount)> BrowseEventsAsync(EventFilterRequest filter)
        {
            var query = _dbContext.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.Status == EventStatus.Published)
                .AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

            if (filter.Date.HasValue)
                query = query.Where(e => e.StartDate.Date == filter.Date.Value.Date);

            if (filter.MaxPrice.HasValue)
                query = query.Where(e => e.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(e => e.Location.Contains(filter.Location));

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(e => e.Title.Contains(filter.SearchTerm) || e.Description.Contains(filter.SearchTerm));

            int totalCount = await query.CountAsync();
            
            var items = await query
                .OrderBy(e => e.StartDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }

    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Ticket?> GetTicketWithDetailsAsync(Guid id)
        {
            return await _dbContext.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetUserTicketsAsync(Guid userId)
        {
            return await _dbContext.Tickets
                .Include(t => t.Event)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.ReservedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserBookedEventAsync(Guid userId, Guid eventId)
        {
            return await _dbContext.Tickets
                .AnyAsync(t => t.UserId == userId && t.EventId == eventId && t.Status != TicketStatus.Cancelled);
        }
    }

    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbContext.Payments
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
    }
}
