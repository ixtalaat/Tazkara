using Tazkara.Application.DTOs.Payment;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentSessionRequest request, Guid userId);
        Task<ApiResponse<PaymentDto>> VerifyPaymentAsync(PaymentVerificationRequest request, Guid userId);
    }
}
