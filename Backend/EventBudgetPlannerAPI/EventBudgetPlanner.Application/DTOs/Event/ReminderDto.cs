namespace EventBudgetPlanner.Application.DTOs.Event
{
    //DTO for creating an email reminder
    public record CreateReminderDto(int EventId, string Email, int DaysBeforeEvent, string? CustomMessage);
}


