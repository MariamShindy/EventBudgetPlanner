namespace EventBudgetPlanner.Application.DTOs
{
    /// <summary>Event data transfer object for read operations</summary>
    public record EventDto(int Id, string Name, DateTime Date, decimal Budget, string? Description, DateTime CreatedDate);
}

