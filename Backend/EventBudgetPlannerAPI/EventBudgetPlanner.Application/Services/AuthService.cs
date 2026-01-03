using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventBudgetPlanner.Application.Services
{
    /// <summary>
    /// Service implementation for authentication and authorization.
    /// Handles user registration, login, and JWT token generation using ASP.NET Core Identity.
    /// Methods are kept small and focused with helper methods for complex operations.
    /// Returns Result objects for clean controller handling.
    /// </summary>
    public class AuthService(
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        IConfiguration _configuration,
        ILogger<AuthService> _logger) : IAuthService
    {

        // Registers a new user account and generates a JWT token.
        // Returns Failure result if user already exists or creation fails.
        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await UserExistsAsync(registerDto.Email))
                return Result<AuthResponseDto>.Failure("Email already in use.");

            var user = CreateNewUser(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return Result<AuthResponseDto>.Failure("Registration failed. " + string.Join(", ", result.Errors.Select(e => e.Description)));

            var authResponse = await GenerateAuthResponseAsync(user);
            return Result<AuthResponseDto>.Success(authResponse);
        }

        // Authenticates user credentials and generates a JWT token if valid.
        // Returns Unauthorized (401) result if credentials are invalid.
        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            var isPasswordValid = await ValidatePasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            var authResponse = await GenerateAuthResponseAsync(user);
            return Result<AuthResponseDto>.Success(authResponse);
        }

        // Gets current user information from claims.
        public Task<Result<object>> GetCurrentUserAsync(string userId, string email, string fullName)
        {
            var userInfo = new { UserId = userId, Email = email, FullName = fullName };
            return Task.FromResult(Result<object>.Success(userInfo));
        }

        // Initiates password reset process by generating a token and sending reset email.
        public async Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                // Don't reveal if user exists for security
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", forgotPasswordDto.Email);
                return Result.Success(); // Return success to prevent email enumeration
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var baseUrl = _configuration["AppSettings:FrontUrl"] ?? _configuration["JWT:Audience"] ?? "http://localhost:4200";
            var resetUrl = $"{baseUrl}/auth/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email ?? "")}";

            try
            {
                await SendPasswordResetEmailAsync(user.Email ?? "", user.FullName, resetUrl);
                _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                return Result.Failure("Failed to send password reset email. Please try again later.");
            }
        }

        // Resets user password using the provided token.
        public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return Result.Failure("Invalid reset token or email.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for {Email}: {Errors}", resetPasswordDto.Email, errors);
                return Result.Failure($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password reset successful for {Email}", resetPasswordDto.Email);
            return Result.Success();
        }

        // Updates user information (FullName and/or Email).
        public async Task<Result<object>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<object>.NotFound("User not found.");

            var changesMade = false;

            // Update FullName if provided
            if (!string.IsNullOrWhiteSpace(updateUserDto.FullName) && user.FullName != updateUserDto.FullName)
            {
                user.FullName = updateUserDto.FullName;
                changesMade = true;
            }

            // Update Email if provided
            if (!string.IsNullOrWhiteSpace(updateUserDto.Email) && user.Email != updateUserDto.Email)
            {
                // Check if new email is already in use
                var existingUser = await _userManager.FindByEmailAsync(updateUserDto.Email);
                if (existingUser != null && existingUser.Id != userId)
                    return Result<object>.Failure("Email is already in use by another account.");

                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email; // Update username to match email
                user.NormalizedEmail = _userManager.NormalizeEmail(updateUserDto.Email);
                user.NormalizedUserName = _userManager.NormalizeName(updateUserDto.Email);
                changesMade = true;
            }

            if (!changesMade)
                return Result<object>.Failure("No changes provided.");

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<object>.Failure($"Failed to update user: {errors}");
            }

            var updatedUserInfo = new { UserId = user.Id, Email = user.Email, FullName = user.FullName };
            _logger.LogInformation("User {UserId} updated successfully", userId);
            return Result<object>.Success(updatedUserInfo);
        }

        #region Private Helper Methods

        //Sends password reset email to user
        private async Task SendPasswordResetEmailAsync(string email, string fullName, string resetUrl)
        {
            var subject = "Password Reset Request - Event Budget Planner";
            var body = $@"
         <!DOCTYPE html>
         <html>
         <head>
             <style>
                 body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                 .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                 .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                 .content {{ padding: 20px; background-color: #f9f9f9; }}
                 .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                 .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
             </style>
         </head>
         <body>
             <div class=""container"">
                 <div class=""header"">
                     <h1>Password Reset Request</h1>
                 </div>
                 <div class=""content"">
                     <p>Hello {fullName},</p>
                     <p>We received a request to reset your password for your Event Budget Planner account.</p>
                     <p>Click the button below to reset your password:</p>
                     <p style=""text-align: center;"">
                         <a href=""{resetUrl}"" class=""button"">Reset Password</a>
                     </p>
                     <p>Or copy and paste this link into your browser:</p>
                     <p style=""word-break: break-all; color: #666;"">{resetUrl}</p>
                     <p><strong>This link will expire in 24 hours.</strong></p>
                     <p>If you didn't request a password reset, please ignore this email.</p>
                 </div>
                 <div class=""footer"">
                     <p>This is an automated email from Event Budget Planner.</p>
                     <p>If you have any questions, please contact support.</p>
                 </div>
             </div>
         </body>
         </html>";

            // Use the email service to send the email
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"] ?? "";
            var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "Event Budget Planner";

            if (string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogWarning("Email SMTP password not configured. Password reset email not sent.");
                throw new InvalidOperationException("Email service not configured.");
            }

            using var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            using var message = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(email);
            await client.SendMailAsync(message);
        }

        //Checks if a user with the specified email already exists
        private async Task<bool> UserExistsAsync(string email) => await _userManager.FindByEmailAsync(email) != null;

        //Creates a new ApplicationUser instance from registration data
        private static ApplicationUser CreateNewUser(RegisterDto registerDto) =>
            new()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                EmailConfirmed = true
            };

        //Validates user password using SignInManager
        private async Task<bool> ValidatePasswordAsync(ApplicationUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded;
        }

        //Generates JWT token and creates authentication response for a user
        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var claims = await BuildUserClaimsAsync(user);
            var (secretKey, issuer, audience, expiryMinutes) = GetJwtSettings();
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var token = CreateJwtToken(claims, secretKey, issuer, audience, expiresAt);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email ?? string.Empty,
                Token: tokenString,
                ExpiresAt: expiresAt);
        }

        //Builds claims list for a user including roles
        private async Task<List<Claim>> BuildUserClaimsAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("FullName", user.FullName)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        //Gets all JWT configuration settings
        private (string secret, string issuer, string audience, int expiryMinutes) GetJwtSettings()
        {
            var secret = GetJwtSetting("Secret");
            var issuer = GetJwtSetting("Issuer");
            var audience = GetJwtSetting("Audience");
            var expiryMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"] ?? "60");
            return (secret, issuer, audience, expiryMinutes);
        }

        //Gets a specific JWT configuration setting with validation
        private string GetJwtSetting(string key) =>
            _configuration[$"JWT:{key}"] ?? throw new InvalidOperationException($"JWT {key} is not configured");

        //Creates a JWT security token with the specified claims and settings
        private static JwtSecurityToken CreateJwtToken(List<Claim> claims, string secretKey, string issuer, string audience, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);
        }

        #endregion
    }
}
