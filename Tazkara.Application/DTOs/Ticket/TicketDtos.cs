using Tazkara.Domain.Enums;

namespace Tazkara.Application.DTOs.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
    }

    public class ReserveTicketRequest
    {
        public Guid EventId { get; set; }
    }
}
