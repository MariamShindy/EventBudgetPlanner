using EventBudgetPlanner.Application.DTOs.Event;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace EventBudgetPlanner.Application.Services
{
    /// <summary>
    /// Service implementation for email operations using SMTP (SendGrid compatible).
    /// For production, configure SendGrid API key in appsettings.json.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        // Sends a reminder email for an event.
        public async Task<Result> SendReminderEmailAsync(CreateReminderDto reminderDto)
        {
            return await SendEventReminderAsync(
                reminderDto.EventId,
                reminderDto.Email,
                reminderDto.DaysBeforeEvent,
                reminderDto.CustomMessage
            );
        }

        // Sends an event reminder email.
        public async Task<Result> SendEventReminderAsync(int eventId, string email, int daysBeforeEvent, string? customMessage = null)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
                if (eventEntity == null)
                    return Result.Failure($"Event with ID {eventId} not found.");

                // Calculate reminder date (for informational purposes)
                var reminderDate = eventEntity.Date.AddDays(-daysBeforeEvent);

                // Warn if event date is in the past (reminder might not be useful)
                if (eventEntity.Date < DateTime.Now)
                {
                    _logger.LogWarning("Sending reminder for past event {EventId} ({EventDate})", eventId, eventEntity.Date);
                }

                // Get event expenses for summary
                var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == eventId);
                var totalSpent = expenses.Sum(e => e.Amount);
                var remainingBudget = eventEntity.Budget - totalSpent;

                // Build email content
                var subject = $"Reminder: {eventEntity.Name} is in {daysBeforeEvent} day(s)";
                var body = BuildReminderEmailBody(eventEntity, daysBeforeEvent, totalSpent, remainingBudget, customMessage);

                // Send email (using SMTP - supports SendGrid, Gmail, or any SMTP server)
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"] ?? "";
                var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? "";
                var fromName = _configuration["Email:FromName"] ?? "Event Budget Planner";

                if (string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email SMTP password not configured. Email not sent.");
                    return Result.Failure("Email service not configured. Please configure SMTP settings in appsettings.json.");
                }

                if (string.IsNullOrEmpty(fromEmail))
                    fromEmail = smtpUser;

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(email);

                await client.SendMailAsync(message);
                _logger.LogInformation("Reminder email sent to {Email} for event {EventId}", email, eventId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reminder email to {Email} for event {EventId}", email, eventId);
                return Result.Failure($"Failed to send email: {ex.Message}");
            }
        }

        private string BuildReminderEmailBody(Domain.Entities.Event eventEntity, int daysBeforeEvent, decimal totalSpent, decimal remainingBudget, string? customMessage)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .event-details {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #4CAF50; }}
        .budget-info {{ background-color: #e8f5e9; padding: 15px; margin: 10px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Event Reminder</h1>
        </div>
        <div class=""content"">
            <h2>Your event is coming up!</h2>
            <p>This is a reminder that <strong>{eventEntity.Name}</strong> is in <strong>{daysBeforeEvent} day(s)</strong>.</p>
            
            <div class=""event-details"">
                <h3>Event Details</h3>
                <p><strong>Date:</strong> {eventEntity.Date:MMMM dd, yyyy 'at' h:mm tt}</p>
                <p><strong>Budget:</strong> {eventEntity.Budget:C}</p>
                {(!string.IsNullOrEmpty(eventEntity.Description) ? $"<p><strong>Description:</strong> {eventEntity.Description}</p>" : "")}
            </div>

            <div class=""budget-info"">
                <h3>Budget Status</h3>
                <p><strong>Total Spent:</strong> {totalSpent:C}</p>
                <p><strong>Remaining Budget:</strong> {remainingBudget:C}</p>
                <p><strong>Budget Used:</strong> {((totalSpent / eventEntity.Budget) * 100):F1}%</p>
            </div>

            {(string.IsNullOrEmpty(customMessage) ? "" : $"<div class=\"event-details\"><p><strong>Note:</strong> {customMessage}</p></div>")}

            <p>Don't forget to review your expenses and budget before the event!</p>
        </div>
        <div class=""footer"">
            <p>This is an automated reminder from Event Budget Planner.</p>
            <p>You can manage your events at <a href=""{_configuration["AppSettings:BaseUrl"] ?? _configuration["JWT:Issuer"] ?? "https://localhost:7199"}"">Event Budget Planner</a></p>
        </div>
    </div>
</body>
</html>";
        }
    }
}


