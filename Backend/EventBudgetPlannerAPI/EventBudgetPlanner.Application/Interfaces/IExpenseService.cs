using EventBudgetPlanner.Application.Common;
using EventBudgetPlanner.Application.DTOs.Expense;

namespace EventBudgetPlanner.Application.Interfaces
{
    /// <summary>Service interface for expense management operations</summary>
    public interface IExpenseService
    {
        Task<Result<IEnumerable<ExpenseDto>>> GetExpensesByEventIdAsync(int eventId, bool? paid, string? category);
        Task<Result<ExpenseDto>> GetExpenseByIdAsync(int id);
        Task<Result<ExpenseDto>> CreateExpenseAsync(CreateExpenseDto createExpenseDto);
        Task<Result> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto);
        Task<Result> DeleteExpenseAsync(int id);
    }
}

