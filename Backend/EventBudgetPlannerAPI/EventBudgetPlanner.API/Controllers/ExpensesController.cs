using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Expense;

namespace EventBudgetPlanner.API.Controllers
{
    //Expenses controller providing CRUD operations and filtering
    [Authorize]
    public class ExpensesController(IExpenseService _expenseService) : ApiController
    {
        //Retrieves all expenses for a specific event with optional filters
        [HttpGet("event/{eventId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesByEventId(int eventId, [FromQuery] bool? paid, [FromQuery] string? category) =>
            (await _expenseService.GetExpensesByEventIdAsync(eventId, paid, category)).ToActionResult();


        //Retrieves a specific expense by ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id) =>
            (await _expenseService.GetExpenseByIdAsync(id)).ToActionResult();

        //Retrieves expenses with advanced filtering
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

        //Creates a new expense
        [HttpPost]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto createExpenseDto) =>
            (await _expenseService.CreateExpenseAsync(createExpenseDto)).ToActionResult();

        //Updates an existing expense
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseDto updateExpenseDto) =>
            (await _expenseService.UpdateExpenseAsync(id, updateExpenseDto)).ToActionResult();

        //Deletes an expense
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExpense(int id) =>
            (await _expenseService.DeleteExpenseAsync(id)).ToActionResult();
    }
}
