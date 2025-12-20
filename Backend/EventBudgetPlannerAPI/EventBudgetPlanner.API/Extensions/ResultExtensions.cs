using EventBudgetPlanner.Application.Common;

namespace EventBudgetPlanner.API.Extensions
{
    /// <summary>Extension methods for converting Result objects to ActionResult responses</summary>
    public static class ResultExtensions
    {
        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            return result.IsSuccess 
                ? new ObjectResult(result.Data) { StatusCode = result.StatusCode }
                : new ObjectResult(new { message = result.Error }) { StatusCode = result.StatusCode };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            return result.IsSuccess
                ? new StatusCodeResult(result.StatusCode)
                : new ObjectResult(new { message = result.Error }) { StatusCode = result.StatusCode };
        }
    }
}



