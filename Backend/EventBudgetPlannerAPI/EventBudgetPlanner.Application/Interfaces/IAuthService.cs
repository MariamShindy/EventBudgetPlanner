namespace EventBudgetPlanner.Application.Interfaces
{
    //Service interface for authentication and JWT token management
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result<object>> GetCurrentUserAsync(string userId, string email, string fullName);
        Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<Result<object>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
    }
}

