namespace EventBudgetPlanner.Application.DTOs
{
    //Event data transfer object for read operations
    public record EventDto(int Id, string Name, DateTime Date, decimal Budget, string? Description, DateTime CreatedDate);
}

