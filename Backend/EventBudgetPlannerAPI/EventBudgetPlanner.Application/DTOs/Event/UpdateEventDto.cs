namespace EventBudgetPlanner.Application.DTOs.Event
{
    //DTO for updating an existing event
    public record UpdateEventDto(string Name, DateTime Date, decimal Budget, string? Description);
}

