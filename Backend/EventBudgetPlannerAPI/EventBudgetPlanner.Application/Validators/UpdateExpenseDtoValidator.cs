using EventBudgetPlanner.Application.DTOs.Expense;

namespace EventBudgetPlanner.Application.Validators
{
    /// <summary>Validator for expense update requests</summary>
    public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
    {
        public UpdateExpenseDtoValidator()
        {
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MinimumLength(2).WithMessage("Category must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.")
                .LessThan(1000000000).WithMessage("Amount value is too large.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateTime.Now.AddYears(1))
                .WithMessage("Expense date cannot be more than 1 year in the future.");
        }
    }
}

