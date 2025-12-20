namespace EventBudgetPlanner.Application.DTOs.Auth
{
    /// <summary>DTO for reset password request</summary>
    public record ResetPasswordDto(string Email, string Token, string NewPassword, string ConfirmPassword);
}

