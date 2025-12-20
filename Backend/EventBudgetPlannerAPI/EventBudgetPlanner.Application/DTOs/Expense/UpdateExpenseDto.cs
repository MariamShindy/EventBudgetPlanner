namespace EventBudgetPlanner.Application.DTOs.Expense
{
    /// <summary>DTO for updating an existing expense</summary>
    public record UpdateExpenseDto(string Category, string? Description, decimal Amount, bool Paid, DateTime Date);
}

