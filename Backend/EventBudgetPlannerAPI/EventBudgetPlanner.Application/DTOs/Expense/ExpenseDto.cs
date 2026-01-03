namespace EventBudgetPlanner.Application.DTOs.Expense
{
    //Expense data transfer object for read operations
    public record ExpenseDto(int Id, int EventId, string Category, string? Description, decimal Amount, bool Paid, DateTime Date, DateTime CreatedDate);
}

