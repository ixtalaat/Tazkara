using Tazkara.Domain.Enums;

namespace Tazkara.Application.Interfaces
{
    public class PaymentGatewayResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public interface IPaymentGateway
    {
        Task<PaymentGatewayResult> CreatePaymentSessionAsync(decimal amount, string referenceId);
        Task<bool> VerifyPaymentAsync(string transactionId, string verificationToken);
    }

    public interface IPaymentGatewayFactory
    {
        IPaymentGateway GetGateway(PaymentProvider provider);
    }
}
