using FluentValidation;
using Tazkara.Application.DTOs.Ticket;

namespace Tazkara.Application.Validators
{
    public class ReserveTicketRequestValidator : AbstractValidator<ReserveTicketRequest>
    {
        public ReserveTicketRequestValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("EventId is required.");
        }
    }
}
