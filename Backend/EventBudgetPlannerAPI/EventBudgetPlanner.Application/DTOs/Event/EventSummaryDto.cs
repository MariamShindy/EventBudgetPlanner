namespace EventBudgetPlanner.Application.DTOs.Event
{
    /// <summary>Event summary DTO with budget tracking and expense analysis</summary>
    public record EventSummaryDto(
        int EventId, 
        string EventName, 
        decimal Budget, 
        decimal TotalSpent, 
        decimal RemainingBudget, 
        decimal PercentageSpent, 
        Dictionary<string, decimal> ExpensesByCategory, 
        int PaidExpensesCount, 
        int UnpaidExpensesCount, 
        int TotalExpensesCount, 
        bool IsOverBudget, 
        decimal OverBudgetAmount);
}

