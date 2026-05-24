using Tazkara.Domain.Enums;

namespace Tazkara.Application.DTOs.Payment
{
    public class PaymentSessionRequest
    {
        public Guid TicketId { get; set; }
        public PaymentProvider Provider { get; set; }
    }

    public class PaymentSessionResponse
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public class PaymentVerificationRequest
    {
        public string TransactionId { get; set; } = string.Empty;
        public string VerificationToken { get; set; } = string.Empty;
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
