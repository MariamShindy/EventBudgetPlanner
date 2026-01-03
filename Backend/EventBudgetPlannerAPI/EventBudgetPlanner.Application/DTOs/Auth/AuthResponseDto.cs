namespace EventBudgetPlanner.Application.DTOs.Auth
{
    //Authentication response DTO containing JWT token and user information
    public record AuthResponseDto(string UserId, string FullName, string Email, string Token, DateTime ExpiresAt);
}

