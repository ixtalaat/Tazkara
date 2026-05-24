using Tazkara.Domain.Enums;

namespace Tazkara.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int AvailableTickets { get; set; }
        public decimal Price { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid OrganizerId { get; set; }
        public Guid CategoryId { get; set; }

        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[]? RowVersion { get; set; }

        // Navigation properties
        public ApplicationUser? Organizer { get; set; }
        public Category? Category { get; set; }
    }
}
