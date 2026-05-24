using Tazkara.Domain.Enums;

namespace Tazkara.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Reserved;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public ApplicationUser? User { get; set; }
        
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
