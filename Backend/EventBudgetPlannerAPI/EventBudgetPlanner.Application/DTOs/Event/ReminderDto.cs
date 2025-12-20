namespace EventBudgetPlanner.Application.DTOs.Event
{
    /// <summary>DTO for creating an email reminder</summary>
    public record CreateReminderDto(int EventId, string Email, int DaysBeforeEvent, string? CustomMessage);

    /// <summary>DTO for reminder response</summary>
    public record ReminderDto(int Id, int EventId, string Email, int DaysBeforeEvent, DateTime ReminderDate, bool IsSent);
}


