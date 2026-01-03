namespace EventBudgetPlanner.Application.DTOs.Event
{

    //DTO for viewing a shared event
    public record SharedEventViewDto(int Id, string Name, DateTime Date, decimal Budget, string? Description, decimal TotalSpent, int ExpenseCount);
}
