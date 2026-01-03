using EventBudgetPlanner.Application.DTOs.Expense;

namespace EventBudgetPlanner.Application.Services
{
    /// <summary>
    /// Service implementation for Expense-related business operations.
    /// Handles expense management logic and data transformation.
    /// Methods are kept small and focused with helper methods for complex operations.
    /// Returns Result objects for clean controller handling.
    /// </summary>
    public class ExpenseService(IUnitOfWork _unitOfWork, IMapper _mapper) : IExpenseService
    {

        // Retrieves expenses for an event with optional filtering by payment status and category.
        // Returns NotFound result if event doesn't exist.
        public async Task<Result<IEnumerable<ExpenseDto>>> GetExpensesByEventIdAsync(int eventId, bool? paid, string? category)
        {
            var eventExists = await _unitOfWork.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
                return Result<IEnumerable<ExpenseDto>>.NotFound($"Event with ID {eventId} not found.");

            var expenses = await GetFilteredExpensesAsync(eventId, paid, category);
            return Result<IEnumerable<ExpenseDto>>.Success(expenses);
        }


        // Retrieves a specific expense by ID.
        // Returns NotFound result if expense doesn't exist.
        public async Task<Result<ExpenseDto>> GetExpenseByIdAsync(int id)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
            if (expense == null)
                return Result<ExpenseDto>.NotFound($"Expense with ID {id} not found.");

            var expenseDto = _mapper.Map<ExpenseDto>(expense);
            return Result<ExpenseDto>.Success(expenseDto);
        }

        // Creates a new expense in the database.
        // Validates that the event exists before creating.
        // Returns 201 Created status on success.
        public async Task<Result<ExpenseDto>> CreateExpenseAsync(CreateExpenseDto createExpenseDto)
        {
            var eventExists = await _unitOfWork.Events.AnyAsync(e => e.Id == createExpenseDto.EventId);
            if (!eventExists)
                return Result<ExpenseDto>.Failure($"Event with ID {createExpenseDto.EventId} does not exist.");

            var expense = _mapper.Map<Expense>(createExpenseDto);
            var createdExpense = await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();
            var expenseDto = _mapper.Map<ExpenseDto>(createdExpense);
            return Result<ExpenseDto>.Success(expenseDto, 201);
        }

        // Updates an existing expense.
        // Returns NotFound result if expense doesn't exist.
        // Returns NoContent (204) on success.
        public async Task<Result> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto)
        {
            var existingExpense = await _unitOfWork.Expenses.GetByIdAsync(id);
            if (existingExpense == null)
                return Result.NotFound($"Expense with ID {id} not found.");

            MapUpdatesToExpense(existingExpense, updateExpenseDto);
            await _unitOfWork.Expenses.UpdateAsync(existingExpense);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(204);
        }

        // Deletes an expense from the database.
        // Returns NotFound result if expense doesn't exist.
        // Returns NoContent (204) on success.
        public async Task<Result> DeleteExpenseAsync(int id)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
            if (expense == null)
                return Result.NotFound($"Expense with ID {id} not found.");

            await _unitOfWork.Expenses.DeleteAsync(expense);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(204);
        }

        #region Private Helper Methods

        // Maps update DTO values to the existing expense entity.
        private void MapUpdatesToExpense(Expense existingExpense, UpdateExpenseDto updateDto)
        {
            _mapper.Map(updateDto, existingExpense);
            existingExpense.ModifiedDate = DateTime.Now;
        }

        // Retrieves expenses with optional filters for payment status and category.
        // Applies filters dynamically based on provided parameters.
        private async Task<IEnumerable<ExpenseDto>> GetFilteredExpensesAsync(int eventId, bool? paid, string? category)
        {
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == eventId);
            var filteredExpenses = ApplyFilters(expenses, paid, category).ToList();
            return filteredExpenses.Select(e => _mapper.Map<ExpenseDto>(e));
        }

        // Applies payment status and category filters to expense list.
        private static IEnumerable<Expense> ApplyFilters(IEnumerable<Expense> expenses, bool? paid, string? category)
        {
            if (paid.HasValue)
                expenses = expenses.Where(e => e.IsPaid == paid.Value);
            
            if (!string.IsNullOrWhiteSpace(category))
                expenses = expenses.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            return expenses;
        }

        #endregion
    }
}
