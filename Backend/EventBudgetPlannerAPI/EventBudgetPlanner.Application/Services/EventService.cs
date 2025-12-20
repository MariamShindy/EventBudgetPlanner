using EventBudgetPlanner.Application.DTOs.Budget;
using EventBudgetPlanner.Application.DTOs.Event;
using Microsoft.Extensions.Configuration;

namespace EventBudgetPlanner.Application.Services
{
    /// <summary>
    /// Service implementation for Event-related business operations.
    /// Handles event management logic and data transformation.
    /// Methods are kept small and focused with helper methods for complex operations.
    /// Returns Result objects for clean controller handling.
    /// </summary>
    public class EventService(IUnitOfWork _unitOfWork, IMapper _mapper, IConfiguration _configuration) : IEventService
    { 
        /// <summary>
        /// Retrieves all events from the database.
        /// </summary>
        public async Task<Result<IEnumerable<EventDto>>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);
            return Result<IEnumerable<EventDto>>.Success(eventDtos);
        }


        /// <summary>
        /// Retrieves a specific event by ID.
        /// Returns NotFound result if event doesn't exist.
        /// </summary>
        public async Task<Result<EventDto>> GetEventByIdAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
                return Result<EventDto>.NotFound($"Event with ID {id} not found.");

            var eventDto = _mapper.Map<EventDto>(eventEntity);
            return Result<EventDto>.Success(eventDto);
        }

        /// <summary>
        /// Creates a new event in the database.
        /// Returns 201 Created status on success.
        /// </summary>
        public async Task<Result<EventDto>> CreateEventAsync(CreateEventDto createEventDto)
        {
            var eventEntity = _mapper.Map<Event>(createEventDto);
            var createdEvent = await _unitOfWork.Events.AddAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();
            var eventDto = _mapper.Map<EventDto>(createdEvent);
            return Result<EventDto>.Success(eventDto, 201);
        }

        /// <summary>
        /// Updates an existing event.
        /// Returns NotFound result if event doesn't exist.
        /// Returns NoContent (204) on success.
        /// </summary>
        public async Task<Result> UpdateEventAsync(int id, UpdateEventDto updateEventDto)
        {
            var existingEvent = await _unitOfWork.Events.GetByIdAsync(id);
            if (existingEvent == null)
                return Result.NotFound($"Event with ID {id} not found.");

            MapUpdatesToEvent(existingEvent, updateEventDto);
            await _unitOfWork.Events.UpdateAsync(existingEvent);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(204);
        }

        /// <summary>
        /// Deletes an event and all its associated expenses.
        /// Returns NotFound result if event doesn't exist.
        /// Returns NoContent (204) on success.
        /// </summary>
        public async Task<Result> DeleteEventAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
                return Result.NotFound($"Event with ID {id} not found.");

            await DeleteAssociatedExpensesAsync(id);
            await _unitOfWork.Events.DeleteAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(204);
        }

        /// <summary>
        /// Generates comprehensive summary information for an event.
        /// Includes budget tracking and expense analysis.
        /// Returns NotFound result if event doesn't exist.
        /// </summary>
        public async Task<Result<EventSummaryDto>> GetEventSummaryAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
                return Result<EventSummaryDto>.NotFound($"Event with ID {id} not found.");

            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == id);
            var summary = BuildEventSummary(eventEntity, expenses.ToList());
            return Result<EventSummaryDto>.Success(summary);
        }

        /// <summary>
        /// Returns cashflow timeseries for an event at the specified interval (week or month).
        /// </summary>
        public async Task<Result<CashflowResponseDto>> GetCashflowAsync(int id, string interval)
        {
            var eventExists = await _unitOfWork.Events.AnyAsync(e => e.Id == id);
            if (!eventExists)
                return Result<CashflowResponseDto>.NotFound($"Event with ID {id} not found.");

            var normalized = (interval ?? "month").Trim().ToLowerInvariant();
            if (normalized != "month" && normalized != "week")
                return Result<CashflowResponseDto>.BadRequest("Interval must be 'week' or 'month'.");

            var expenses = (await _unitOfWork.Expenses.FindAsync(e => e.EventId == id)).ToList();
            if (expenses.Count == 0)
                return Result<CashflowResponseDto>.Success(new CashflowResponseDto(id, normalized, new List<CashflowPointDto>()));

            var minDate = expenses.Min(e => e.Date).Date;
            var maxDate = expenses.Max(e => e.Date).Date;

            DateTime cursor = normalized == "week" ? StartOfWeek(minDate) : new DateTime(minDate.Year, minDate.Month, 1);
            var endBoundary = normalized == "week" ? StartOfWeek(maxDate).AddDays(7).AddDays(-1) : new DateTime(maxDate.Year, maxDate.Month, 1).AddMonths(1).AddDays(-1);

            var points = new List<CashflowPointDto>();
            decimal cumulative = 0m;
            while (cursor <= endBoundary)
            {
                DateTime periodStart = cursor;
                DateTime periodEnd = normalized == "week"
                    ? periodStart.AddDays(7).AddTicks(-1)
                    : periodStart.AddMonths(1).AddTicks(-1);

                var total = expenses.Where(e => e.Date >= periodStart && e.Date <= periodEnd).Sum(e => e.Amount);
                cumulative += total;
                points.Add(new CashflowPointDto(periodStart, periodEnd, total, cumulative));

                cursor = normalized == "week" ? periodStart.AddDays(7) : periodStart.AddMonths(1);
            }

            return Result<CashflowResponseDto>.Success(new CashflowResponseDto(id, normalized, points));
        }

        /// <summary>
        /// Allocates an event's total budget into category planned amounts using the chosen strategy.
        /// Persists allocations in EventCategoryBudgets (upsert).
        /// </summary>
        public async Task<Result<AllocationResponseDto>> AllocateBudgetAsync(int id, AllocateBudgetRequestDto request)
        {
            if (request.TotalBudget <= 0)
                return Result<AllocationResponseDto>.BadRequest("totalBudget must be > 0");

            var evt = await _unitOfWork.Events.GetByIdAsync(id);
            if (evt == null)
                return Result<AllocationResponseDto>.NotFound($"Event with ID {id} not found.");

            var strategy = (request.Strategy ?? "equal").Trim().ToLowerInvariant();
            if (strategy != "equal" && strategy != "templateweighted")
                return Result<AllocationResponseDto>.BadRequest("strategy must be 'equal' or 'templateWeighted'");

            // Determine categories
            List<string> categories;
            Dictionary<string, decimal>? weights = null;
            if (strategy == "templateweighted" && evt.EventTemplateId.HasValue)
            {
                var templateId = evt.EventTemplateId.Value;
                var templateCats = await _unitOfWork.EventTemplateCategories.FindAsync(c => c.EventTemplateId == templateId);
                categories = templateCats.Select(c => c.CategoryName).Distinct().ToList();
                var sumEst = templateCats.Sum(c => c.EstimatedAmount);
                if (sumEst > 0)
                    weights = templateCats.ToDictionary(c => c.CategoryName, c => c.EstimatedAmount / sumEst);
            }
            else
            {
                // Fall back to categories observed in expenses
                var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == id);
                categories = expenses.Select(e => e.Category).Distinct().OrderBy(x => x).ToList();
            }

            if (categories.Count == 0)
                return Result<AllocationResponseDto>.BadRequest("No categories available to allocate. Add expenses or link a template with categories.");

            var perCategory = new List<AllocationItemDto>();
            if (weights != null)
            {
                foreach (var cat in categories)
                {
                    var w = weights.TryGetValue(cat, out var v) ? v : 0m;
                    perCategory.Add(new AllocationItemDto(cat, Math.Round(request.TotalBudget * w, 2)));
                }
            }
            else
            {
                var equal = Math.Round(request.TotalBudget / categories.Count, 2);
                foreach (var cat in categories)
                    perCategory.Add(new AllocationItemDto(cat, equal));
            }

            // Persist allocations (upsert)
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Load existing allocations
                var existing = await _unitOfWork.EventCategoryBudgets.FindAsync(a => a.EventId == id);
                var existingMap = existing.ToDictionary(a => a.Category, StringComparer.OrdinalIgnoreCase);

                foreach (var item in perCategory)
                {
                    if (existingMap.TryGetValue(item.Category, out var alloc))
                    {
                        alloc.PlannedAmount = item.PlannedAmount;
                        await _unitOfWork.EventCategoryBudgets.UpdateAsync(alloc);
                    }
                    else
                    {
                        await _unitOfWork.EventCategoryBudgets.AddAsync(new EventCategoryBudget
                        {
                            EventId = id,
                            Category = item.Category,
                            PlannedAmount = item.PlannedAmount
                        });
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return Result<AllocationResponseDto>.Success(new AllocationResponseDto(id, request.TotalBudget, strategy, perCategory));
        }

        /// <summary>
        /// Generates a shareable link for an event.
        /// Creates a unique token if one doesn't exist.
        /// </summary>
        public async Task<Result<ShareEventDto>> GenerateShareLinkAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
                return Result<ShareEventDto>.NotFound($"Event with ID {id} not found.");

            // Generate token if it doesn't exist
            if (string.IsNullOrEmpty(eventEntity.ShareToken))
            {
                eventEntity.ShareToken = Guid.NewGuid().ToString("N")[..16]; // 16 character token
                await _unitOfWork.Events.UpdateAsync(eventEntity);
                await _unitOfWork.SaveChangesAsync();
            }

            var baseUrl = _configuration["AppSettings:FrontUrl"] ?? _configuration["JWT:Audiences"] ?? "http://localhost:4200";
            var shareUrl = $"{baseUrl}/share/{eventEntity.ShareToken}";
            return Result<ShareEventDto>.Success(new ShareEventDto(shareUrl, eventEntity.ShareToken));
        }

        /// <summary>
        /// Retrieves a shared event by its share token (public endpoint, no auth required).
        /// </summary>
        public async Task<Result<SharedEventViewDto>> GetSharedEventAsync(string shareToken)
        {
            var eventEntity = (await _unitOfWork.Events.FindAsync(e => e.ShareToken == shareToken)).FirstOrDefault();
            if (eventEntity == null)
                return Result<SharedEventViewDto>.NotFound("Shared event not found or invalid token.");

            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == eventEntity.Id);
            var totalSpent = expenses.Sum(e => e.Amount);
            var expenseCount = expenses.Count();

            var sharedView = new SharedEventViewDto(
                eventEntity.Id,
                eventEntity.Name,
                eventEntity.Date,
                eventEntity.Budget,
                eventEntity.Description,
                totalSpent,
                expenseCount
            );

            return Result<SharedEventViewDto>.Success(sharedView);
        }

        /// <summary>
        /// Retrieves all template events that users can copy.
        /// </summary>
        public async Task<Result<IEnumerable<EventDto>>> GetTemplateEventsAsync()
        {
            var templateEvents = await _unitOfWork.Events.FindAsync(e => e.IsTemplate);
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(templateEvents);
            return Result<IEnumerable<EventDto>>.Success(eventDtos);
        }

        #region Private Helper Methods

        /// <summary>
        /// Maps update DTO values to the existing event entity.
        /// </summary>
        /// <param name="existingEvent">The event entity to update</param>
        /// <param name="updateDto">The DTO containing updated values</param>
        private void MapUpdatesToEvent(Event existingEvent, UpdateEventDto updateDto)
        {
            _mapper.Map(updateDto, existingEvent);
            existingEvent.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Deletes all expenses associated with an event.
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        private async Task DeleteAssociatedExpensesAsync(int eventId)
        {
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.EventId == eventId);
            foreach (var expense in expenses)
                await _unitOfWork.Expenses.DeleteAsync(expense);
        }

        /// <summary>
        /// Builds a comprehensive event summary from event and expense data.
        /// </summary>
        /// <param name="eventEntity">The event entity</param>
        /// <param name="expenses">List of expenses for the event</param>
        /// <returns>Event summary DTO with calculated metrics</returns>
        private EventSummaryDto BuildEventSummary(Event eventEntity, List<Expense> expenses)
        {
            var totalSpent = CalculateTotalSpent(expenses);
            var remainingBudget = CalculateRemainingBudget(eventEntity.Budget, totalSpent);
            var percentageSpent = CalculatePercentageSpent(eventEntity.Budget, totalSpent);
            var expensesByCategory = GroupExpensesByCategory(expenses);
            var (paidCount, unpaidCount) = CountPaidAndUnpaidExpenses(expenses);
            var (isOverBudget, overBudgetAmount) = CalculateOverBudgetStatus(eventEntity.Budget, totalSpent);

            return new EventSummaryDto(
                EventId: eventEntity.Id,
                EventName: eventEntity.Name,
                Budget: eventEntity.Budget,
                TotalSpent: totalSpent,
                RemainingBudget: remainingBudget,
                PercentageSpent: Math.Round(percentageSpent, 2),
                ExpensesByCategory: expensesByCategory,
                PaidExpensesCount: paidCount,
                UnpaidExpensesCount: unpaidCount,
                TotalExpensesCount: expenses.Count,
                IsOverBudget: isOverBudget,
                OverBudgetAmount: overBudgetAmount);
        }

        /// <summary>Calculates the total amount spent across all expenses.</summary>
        private static decimal CalculateTotalSpent(List<Expense> expenses) => expenses.Sum(e => e.Amount);

        /// <summary>Calculates the remaining budget.</summary>
        private static decimal CalculateRemainingBudget(decimal budget, decimal totalSpent) => budget - totalSpent;

        /// <summary>Calculates the percentage of budget that has been spent.</summary>
        private static decimal CalculatePercentageSpent(decimal budget, decimal totalSpent) => budget > 0 ? (totalSpent / budget) * 100 : 0;

        /// <summary>Groups expenses by category and calculates totals for each.</summary>
        private static Dictionary<string, decimal> GroupExpensesByCategory(List<Expense> expenses) =>
            expenses.GroupBy(e => e.Category).ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        /// <summary>Counts paid and unpaid expenses.</summary>
        private static (int paidCount, int unpaidCount) CountPaidAndUnpaidExpenses(List<Expense> expenses)
        {
            var paidCount = expenses.Count(e => e.IsPaid);
            var unpaidCount = expenses.Count(e => !e.IsPaid);
            return (paidCount, unpaidCount);
        }

        /// <summary>Determines if the budget is exceeded and by how much.</summary>
        private static (bool isOverBudget, decimal overBudgetAmount) CalculateOverBudgetStatus(decimal budget, decimal totalSpent)
        {
            var isOverBudget = totalSpent > budget;
            var overBudgetAmount = isOverBudget ? totalSpent - budget : 0;
            return (isOverBudget, overBudgetAmount);
        }

        /// <summary>Gets the start of the week (Monday) for a given date.</summary>
        private static DateTime StartOfWeek(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        #endregion
    }
}
