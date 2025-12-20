namespace EventBudgetPlanner.Application.DTOs
{
    /// <summary>DTO for sharing an event</summary>
    public record ShareEventDto(string ShareUrl, string ShareToken);

    /// <summary>DTO for viewing a shared event</summary>
    public record SharedEventViewDto(int Id, string Name, DateTime Date, decimal Budget, string? Description, decimal TotalSpent, int ExpenseCount);
}


