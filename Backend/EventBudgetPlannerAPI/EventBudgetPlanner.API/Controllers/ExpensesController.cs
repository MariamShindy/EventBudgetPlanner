using EventBudgetPlanner.API.Extensions;

namespace EventBudgetPlanner.API.Controllers
{
    /// <summary>Expenses controller providing CRUD operations and filtering (requires authentication)</summary>
    [Authorize]
    public class ExpensesController(IExpenseService _expenseService) : ApiController
    {
        /// <summary>Retrieves all expenses for a specific event with optional filters</summary>
        [HttpGet("event/{eventId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesByEventId(int eventId, [FromQuery] bool? paid, [FromQuery] string? category) =>
            (await _expenseService.GetExpensesByEventIdAsync(eventId, paid, category)).ToActionResult();


        /// <summary>Retrieves a specific expense by ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id) =>
            (await _expenseService.GetExpenseByIdAsync(id)).ToActionResult();

        /// <summary>Retrieves expenses with advanced filtering</summary>
        [HttpPost("filter")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesWithFilter([FromBody] ExpenseFilterDto filter)
        {
            var expenses = await _expenseService.GetExpensesByEventIdAsync(filter.EventId ?? 0, filter.IsPaid, filter.Category);
            if (!expenses.IsSuccess)
                return BadRequest(expenses.Error);

            var filteredExpenses = (expenses.Data ?? Enumerable.Empty<ExpenseDto>()).AsQueryable();

            // Apply additional filters
            if (filter.MinAmount.HasValue)
                filteredExpenses = filteredExpenses.Where(e => e.Amount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                filteredExpenses = filteredExpenses.Where(e => e.Amount <= filter.MaxAmount.Value);

            if (filter.StartDate.HasValue)
                filteredExpenses = filteredExpenses.Where(e => e.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                filteredExpenses = filteredExpenses.Where(e => e.Date <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                filteredExpenses = filteredExpenses.Where(e => 
                    e.Category.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (e.Description != null && e.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            // Apply pagination
            var pagedExpenses = filteredExpenses
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return Ok(pagedExpenses);
        }

        /// <summary>Creates a new expense</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto createExpenseDto) =>
            (await _expenseService.CreateExpenseAsync(createExpenseDto)).ToActionResult();

        /// <summary>Updates an existing expense</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseDto updateExpenseDto) =>
            (await _expenseService.UpdateExpenseAsync(id, updateExpenseDto)).ToActionResult();

        /// <summary>Deletes an expense</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExpense(int id) =>
            (await _expenseService.DeleteExpenseAsync(id)).ToActionResult();
    }
}
