namespace EventBudgetPlanner.Application.DTOs.Expense
{
    //DTO for updating an existing expense
    public record UpdateExpenseDto(string Category, string? Description, decimal Amount, bool Paid, DateTime Date);
}

