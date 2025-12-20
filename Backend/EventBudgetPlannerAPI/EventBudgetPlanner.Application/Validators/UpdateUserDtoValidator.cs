namespace EventBudgetPlanner.Application.Validators
{
    /// <summary>Validator for update user requests</summary>
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.FullName) || !string.IsNullOrEmpty(x.Email))
                .WithMessage("At least one field (FullName or Email) must be provided.");
        }
    }
}

