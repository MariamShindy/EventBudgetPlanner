namespace EventBudgetPlanner.API.Controllers
{
    /// <summary>Base API controller with common configuration for all controllers</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
    }
}
