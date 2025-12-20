using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.Application.Interfaces
{
    /// <summary>Service interface for email operations</summary>
    public interface IEmailService
    {
        Task<Result> SendReminderEmailAsync(CreateReminderDto reminderDto);
        Task<Result> SendEventReminderAsync(int eventId, string email, int daysBeforeEvent, string? customMessage = null);
    }
}


