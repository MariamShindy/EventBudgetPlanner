using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs;
using EventBudgetPlanner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EventBudgetPlanner.API.Controllers
{
    /// <summary>Share controller for public event sharing (no authentication required)</summary>
    [AllowAnonymous]
    public class ShareController(IEventService _eventService) : ApiController
    {
        /// <summary>Retrieves a shared event by its share token (public endpoint)</summary>
        [HttpGet("{shareToken}")]
        [ProducesResponseType(typeof(SharedEventViewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SharedEventViewDto>> GetSharedEvent(string shareToken) =>
            (await _eventService.GetSharedEventAsync(shareToken)).ToActionResult();
    }
}

