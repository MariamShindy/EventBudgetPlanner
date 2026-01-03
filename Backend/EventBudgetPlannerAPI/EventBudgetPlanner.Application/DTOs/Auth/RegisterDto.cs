namespace EventBudgetPlanner.Application.DTOs.Auth
{
    //DTO for user registration
    public record RegisterDto(string FullName, string Email, string Password, string ConfirmPassword);
}

