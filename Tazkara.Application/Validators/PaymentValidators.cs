using FluentValidation;
using Tazkara.Application.DTOs.Payment;

namespace Tazkara.Application.Validators
{
    public class PaymentSessionRequestValidator : AbstractValidator<PaymentSessionRequest>
    {
        public PaymentSessionRequestValidator()
        {
            RuleFor(x => x.TicketId).NotEmpty().WithMessage("TicketId is required.");
            RuleFor(x => x.Provider).IsInEnum().WithMessage("Invalid Payment Provider.");
        }
    }

    public class PaymentVerificationRequestValidator : AbstractValidator<PaymentVerificationRequest>
    {
        public PaymentVerificationRequestValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty().WithMessage("TransactionId is required.");
            RuleFor(x => x.VerificationToken).NotEmpty().WithMessage("VerificationToken is required.");
        }
    }
}
