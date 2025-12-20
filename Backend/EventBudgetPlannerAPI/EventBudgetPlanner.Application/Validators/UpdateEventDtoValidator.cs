using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.Application.Validators
{
    /// <summary>Validator for event update requests</summary>
    public class UpdateEventDtoValidator : AbstractValidator<UpdateEventDto>
    {
        public UpdateEventDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Event name is required.")
                .MinimumLength(3).WithMessage("Event name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Event name cannot exceed 200 characters.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Event date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Event date cannot be in the past.");

            RuleFor(x => x.Budget)
                .GreaterThan(0).WithMessage("Budget must be greater than zero.")
                .LessThan(1000000000).WithMessage("Budget value is too large.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}

