using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Budget;

namespace EventBudgetPlanner.API.Controllers
{
    /// <summary>Events controller providing CRUD operations and budget summaries (requires authentication)</summary>
    [Authorize]
    public class EventsController (IEventService _eventService) : ApiController
    {
        /// <summary>Retrieves all events</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents() =>
            (await _eventService.GetAllEventsAsync()).ToActionResult();


        /// <summary>Retrieves a specific event by ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetEventById(int id) =>
            (await _eventService.GetEventByIdAsync(id)).ToActionResult();

        /// <summary>Retrieves events with advanced filtering</summary>
        [HttpPost("filter")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsWithFilter([FromBody] EventFilterDto filter)
       {
            var events = await _eventService.GetAllEventsAsync();
            if (!events.IsSuccess)
                return BadRequest(events.Error);

            var filteredEvents = (events.Data ?? Enumerable.Empty<EventDto>()).AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                filteredEvents = filteredEvents.Where(e => 
                    e.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (e.Description != null && e.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (filter.StartDate.HasValue)
                filteredEvents = filteredEvents.Where(e => e.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                filteredEvents = filteredEvents.Where(e => e.Date <= filter.EndDate.Value);

            if (filter.MinBudget.HasValue)
                filteredEvents = filteredEvents.Where(e => e.Budget >= filter.MinBudget.Value);

            if (filter.MaxBudget.HasValue)
                filteredEvents = filteredEvents.Where(e => e.Budget <= filter.MaxBudget.Value);

            // Apply pagination
            var pagedEvents = filteredEvents
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return Ok(pagedEvents);
        }

        /// <summary>Creates a new event</summary>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto) =>
            (await _eventService.CreateEventAsync(createEventDto)).ToActionResult();

        /// <summary>Updates an existing event</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto updateEventDto) =>
            (await _eventService.UpdateEventAsync(id, updateEventDto)).ToActionResult();

        /// <summary>Deletes an event and all its associated expenses</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id) =>
            (await _eventService.DeleteEventAsync(id)).ToActionResult();

        /// <summary>Retrieves comprehensive summary for an event including budget analysis</summary>
        [HttpGet("{id:int}/summary")]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventSummaryDto>> GetEventSummary(int id) =>
            (await _eventService.GetEventSummaryAsync(id)).ToActionResult();

        /// <summary>Returns cashflow timeseries for an event at the specified interval (week or month)</summary>
        [HttpGet("{id:int}/cashflow")]
        [ProducesResponseType(typeof(CashflowResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CashflowResponseDto>> GetCashflow(int id, [FromQuery] string interval = "month") =>
            (await _eventService.GetCashflowAsync(id, interval)).ToActionResult();

        /// <summary>Allocates event budget into categories using a strategy and persists planned amounts</summary>
        [HttpPost("{id:int}/budget/allocate")]
        [ProducesResponseType(typeof(AllocationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AllocationResponseDto>> AllocateBudget(int id, [FromBody] AllocateBudgetRequestDto request) =>
            (await _eventService.AllocateBudgetAsync(id, request)).ToActionResult();

        /// <summary>Generates a shareable link for an event</summary>
        [HttpPost("{id:int}/share")]
        [ProducesResponseType(typeof(ShareEventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShareEventDto>> GenerateShareLink(int id) =>
            (await _eventService.GenerateShareLinkAsync(id)).ToActionResult();

        /// <summary>Retrieves template events that users can copy</summary>
        [HttpGet("templates")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetTemplateEvents() =>
            (await _eventService.GetTemplateEventsAsync()).ToActionResult();
    }
}
