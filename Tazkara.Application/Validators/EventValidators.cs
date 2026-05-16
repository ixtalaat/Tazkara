using FluentValidation;
using Tazkara.Application.DTOs.Event;

namespace Tazkara.Application.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.StartDate).GreaterThan(DateTime.UtcNow).WithMessage("Start Date must be in the future.");
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End Date must be after Start Date.");
        }
    }

    public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
    {
        public UpdateEventRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.StartDate).GreaterThan(DateTime.UtcNow).WithMessage("Start Date must be in the future.");
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End Date must be after Start Date.");
        }
    }
}
