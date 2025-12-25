using EventBudgetPlanner.API.Extensions;
using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.API.Controllers
{
    /// <summary>Reminders controller for email reminders (requires authentication)</summary>
    [Authorize]
    public class RemindersController(IEmailService _emailService) : ApiController
    {
        /// <summary>Sends an email reminder for an event</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendReminder([FromBody] CreateReminderDto reminderDto) =>
            (await _emailService.SendReminderEmailAsync(reminderDto)).ToActionResult();
    }
}


