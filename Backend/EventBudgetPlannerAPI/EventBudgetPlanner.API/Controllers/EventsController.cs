using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Budget;
using EventBudgetPlanner.Application.DTOs.Event;
using EventBudgetPlanner.Application.DTOs.Expense;

namespace EventBudgetPlanner.API.Controllers
{
    //Events controller providing CRUD operations and budget summaries
    [Authorize]
    public class EventsController (IEventService _eventService) : ApiController
    {
        //Retrieves all events
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents() =>
            (await _eventService.GetAllEventsAsync()).ToActionResult();


        //Retrieves a specific event by ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetEventById(int id) =>
            (await _eventService.GetEventByIdAsync(id)).ToActionResult();

        //Retrieves events with advanced filtering
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

        //Creates a new event
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto) =>
            (await _eventService.CreateEventAsync(createEventDto)).ToActionResult();

        //Updates an existing event
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto updateEventDto) =>
            (await _eventService.UpdateEventAsync(id, updateEventDto)).ToActionResult();

        //Deletes an event and all its associated expenses
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id) =>
            (await _eventService.DeleteEventAsync(id)).ToActionResult();

        //Retrieves comprehensive summary for an event including budget analysis
        [HttpGet("{id:int}/summary")]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventSummaryDto>> GetEventSummary(int id) =>
            (await _eventService.GetEventSummaryAsync(id)).ToActionResult();

        //Returns cashflow timeseries for an event at the specified interval (week or month)
        [HttpGet("{id:int}/cashflow")]
        [ProducesResponseType(typeof(CashflowResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CashflowResponseDto>> GetCashflow(int id, [FromQuery] string interval = "month") =>
            (await _eventService.GetCashflowAsync(id, interval)).ToActionResult();

        //Allocates event budget into categories using a strategy and persists planned amounts
        [HttpPost("{id:int}/budget/allocate")]
        [ProducesResponseType(typeof(AllocationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AllocationResponseDto>> AllocateBudget(int id, [FromBody] AllocateBudgetRequestDto request) =>
            (await _eventService.AllocateBudgetAsync(id, request)).ToActionResult();

        //Generates a shareable link for an event
        [HttpPost("{id:int}/share")]
        [ProducesResponseType(typeof(ShareEventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShareEventDto>> GenerateShareLink(int id) =>
            (await _eventService.GenerateShareLinkAsync(id)).ToActionResult();

        //Retrieves template events that users can copy
        [HttpGet("templates")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetTemplateEvents() =>
            (await _eventService.GetTemplateEventsAsync()).ToActionResult();
    }
}
