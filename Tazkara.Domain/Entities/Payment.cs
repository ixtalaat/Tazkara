using Tazkara.Domain.Enums;

namespace Tazkara.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentProvider Provider { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public Guid TicketId { get; set; }

        // Navigation property
        public Ticket? Ticket { get; set; }
    }
}
