namespace EventBudgetPlanner.API.Controllers
{
    //>Base API controller with common configuration for all controllers
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
    }
}
