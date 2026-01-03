using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Auth;

namespace EventBudgetPlanner.API.Controllers
{
    //Authentication controller for user registration and login
    public class AuthController(IAuthService _authService) : ApiController
    {
        //Registers a new user account
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto) =>
            (await _authService.RegisterAsync(registerDto)).ToActionResult();

        //Authenticates a user and returns a JWT token
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto) =>
            (await _authService.LoginAsync(loginDto)).ToActionResult();

        //Validates the current user's authentication status
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<object>> GetCurrentUser() =>
            (await _authService.GetCurrentUserAsync(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
                User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                User.FindFirst("FullName")?.Value ?? "")).ToActionResult();

        //Initiates password reset by sending email with reset token
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto) =>
            (await _authService.ForgotPasswordAsync(forgotPasswordDto)).ToActionResult();

        //Resets user password using the provided token
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto) =>
            (await _authService.ResetPasswordAsync(resetPasswordDto)).ToActionResult();

        //Updates current user information (FullName and/or Email)
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> UpdateUser([FromBody] UpdateUserDto updateUserDto) =>
            (await _authService.UpdateUserAsync(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
                updateUserDto)).ToActionResult();
    }
}
