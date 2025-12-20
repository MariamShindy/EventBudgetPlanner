namespace EventBudgetPlanner.Application.DTOs.Auth
{
    /// <summary>Authentication response DTO containing JWT token and user information</summary>
    public record AuthResponseDto(string UserId, string FullName, string Email, string Token, DateTime ExpiresAt);
}

