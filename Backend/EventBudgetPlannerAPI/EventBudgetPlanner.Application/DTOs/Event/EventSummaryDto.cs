namespace EventBudgetPlanner.Application.DTOs.Event
{
    //Event summary DTO with budget tracking and expense analysis
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

