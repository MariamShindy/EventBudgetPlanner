using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.Application.Interfaces
{
    //Service interface for email operations
    public interface IEmailService
    {
        Task<Result> SendReminderEmailAsync(CreateReminderDto reminderDto);
        Task<Result> SendEventReminderAsync(int eventId, string email, int daysBeforeEvent, string? customMessage = null);
    }
}


