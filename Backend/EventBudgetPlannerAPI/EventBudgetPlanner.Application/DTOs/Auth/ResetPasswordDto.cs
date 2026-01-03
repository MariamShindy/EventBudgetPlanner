namespace EventBudgetPlanner.Application.DTOs.Auth
{
    //DTO for reset password request
    public record ResetPasswordDto(string Email, string Token, string NewPassword, string ConfirmPassword);
}

