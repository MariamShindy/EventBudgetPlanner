namespace EventBudgetPlanner.Application.DTOs.Event
{
    /// <summary>DTO for updating an existing event</summary>
    public record UpdateEventDto(string Name, DateTime Date, decimal Budget, string? Description);
}

