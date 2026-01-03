namespace EventBudgetPlanner.Application.DTOs.Expense
{
    //DTO for creating a new expense
    public record CreateExpenseDto(
        int EventId, 
        string Category, 
        string? Description, 
        decimal Amount, 
        string CurrencyCode = "USD",
        bool IsPaid = false, 
        DateTime? Date = null,
        string? Vendor = null,
        string? ReceiptImagePath = null,
        string? ReceiptFileName = null);
}

