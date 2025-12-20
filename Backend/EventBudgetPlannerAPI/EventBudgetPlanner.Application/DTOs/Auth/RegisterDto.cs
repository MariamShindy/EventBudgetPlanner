namespace EventBudgetPlanner.Application.DTOs.Auth
{
    /// <summary>DTO for user registration</summary>
    public record RegisterDto(string FullName, string Email, string Password, string ConfirmPassword);
}

