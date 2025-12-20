namespace EventBudgetPlanner.Application.DTOs.Expense
{
    /// <summary>Expense data transfer object for read operations</summary>
    public record ExpenseDto(int Id, int EventId, string Category, string? Description, decimal Amount, bool Paid, DateTime Date, DateTime CreatedDate);
}

