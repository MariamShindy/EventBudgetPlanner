using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.API.Controllers
{
    //Share controller for public event sharing
    [AllowAnonymous]
    public class ShareController(IEventService _eventService) : ApiController
    {
        //Retrieves a shared event by its share token
        [HttpGet("{shareToken}")]
        [ProducesResponseType(typeof(SharedEventViewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SharedEventViewDto>> GetSharedEvent(string shareToken) =>
            (await _eventService.GetSharedEventAsync(shareToken)).ToActionResult();
    }
}

