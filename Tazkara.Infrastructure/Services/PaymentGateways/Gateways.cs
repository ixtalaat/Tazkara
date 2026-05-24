using Tazkara.Application.Interfaces;
using Tazkara.Domain.Enums;

namespace Tazkara.Infrastructure.Services.PaymentGateways
{
    public class PayPalGateway : IPaymentGateway
    {
        public async Task<PaymentGatewayResult> CreatePaymentSessionAsync(decimal amount, string referenceId)
        {
            // Simulate PayPal order creation
            await Task.Delay(50);
            return new PaymentGatewayResult
            {
                Success = true,
                TransactionId = $"PAYPAL-{Guid.NewGuid()}",
                PaymentUrl = $"https://sandbox.paypal.com/checkoutnow?token={Guid.NewGuid()}"
            };
        }

        public async Task<bool> VerifyPaymentAsync(string transactionId, string verificationToken)
        {
            // Simulate PayPal order capture
            await Task.Delay(50);
            return true; // Simulate success
        }
    }

    public class VodafoneCashGateway : IPaymentGateway
    {
        public async Task<PaymentGatewayResult> CreatePaymentSessionAsync(decimal amount, string referenceId)
        {
            // Simulate Vodafone Cash or Paymob iframe generation
            await Task.Delay(50);
            return new PaymentGatewayResult
            {
                Success = true,
                TransactionId = $"VF-{Guid.NewGuid()}",
                PaymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/12345?payment_token={Guid.NewGuid()}"
            };
        }

        public async Task<bool> VerifyPaymentAsync(string transactionId, string verificationToken)
        {
            // Simulate verifying webhook/callback signature
            await Task.Delay(50);
            return true; // Simulate success
        }
    }

    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGateway GetGateway(PaymentProvider provider)
        {
            return provider switch
            {
                PaymentProvider.PayPal => (IPaymentGateway)_serviceProvider.GetService(typeof(PayPalGateway))!,
                PaymentProvider.VodafoneCash => (IPaymentGateway)_serviceProvider.GetService(typeof(VodafoneCashGateway))!,
                _ => throw new ArgumentException("Unsupported payment provider")
            };
        }
    }
}
