using Mapster;
using Tazkara.Application.DTOs.Payment;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;

namespace Tazkara.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentGatewayFactory _gatewayFactory;

        public PaymentService(
            IPaymentRepository paymentRepository, 
            ITicketRepository ticketRepository, 
            IPaymentGatewayFactory gatewayFactory)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _gatewayFactory = gatewayFactory;
        }

        public async Task<ApiResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentSessionRequest request, Guid userId)
        {
            var ticket = await _ticketRepository.GetTicketWithDetailsAsync(request.TicketId);
            if (ticket == null || ticket.UserId != userId)
                return ApiResponse<PaymentSessionResponse>.ErrorResponse("Ticket not found.");

            if (ticket.Status == TicketStatus.Cancelled)
                return ApiResponse<PaymentSessionResponse>.ErrorResponse("Cannot pay for a cancelled ticket.");

            if (ticket.PaymentStatus == PaymentStatus.Paid)
                return ApiResponse<PaymentSessionResponse>.ErrorResponse("Ticket is already paid.");

            var gateway = _gatewayFactory.GetGateway(request.Provider);
            var result = await gateway.CreatePaymentSessionAsync(ticket.Event!.Price, ticket.TicketNumber);

            if (!result.Success)
                return ApiResponse<PaymentSessionResponse>.ErrorResponse($"Failed to create payment session: {result.ErrorMessage}");

            var payment = new Payment
            {
                TicketId = ticket.Id,
                Amount = ticket.Event.Price,
                TransactionId = result.TransactionId,
                Provider = request.Provider,
                Status = PaymentStatus.Pending
            };

            await _paymentRepository.AddAsync(payment);

            var response = new PaymentSessionResponse
            {
                PaymentUrl = result.PaymentUrl,
                TransactionId = result.TransactionId
            };

            return ApiResponse<PaymentSessionResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<PaymentDto>> VerifyPaymentAsync(PaymentVerificationRequest request, Guid userId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(request.TransactionId);
            if (payment == null)
                return ApiResponse<PaymentDto>.ErrorResponse("Payment not found.");

            if (payment.Ticket!.UserId != userId)
                return ApiResponse<PaymentDto>.ErrorResponse("Unauthorized access to payment.");

            if (payment.Status == PaymentStatus.Paid)
                return ApiResponse<PaymentDto>.ErrorResponse("Payment already verified.");

            var gateway = _gatewayFactory.GetGateway(payment.Provider);
            var verified = await gateway.VerifyPaymentAsync(request.TransactionId, request.VerificationToken);

            if (!verified)
            {
                payment.Status = PaymentStatus.Failed;
                await _paymentRepository.UpdateAsync(payment);
                return ApiResponse<PaymentDto>.ErrorResponse("Payment verification failed.");
            }

            payment.Status = PaymentStatus.Paid;
            await _paymentRepository.UpdateAsync(payment);

            payment.Ticket.Status = TicketStatus.Confirmed;
            payment.Ticket.PaymentStatus = PaymentStatus.Paid;
            await _ticketRepository.UpdateAsync(payment.Ticket);

            return ApiResponse<PaymentDto>.SuccessResponse(payment.Adapt<PaymentDto>());
        }
    }
}
