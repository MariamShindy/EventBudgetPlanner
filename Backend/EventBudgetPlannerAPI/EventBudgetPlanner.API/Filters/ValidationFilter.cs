using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventBudgetPlanner.API.Filters
{
    /// <summary>Action filter for automatic FluentValidation before controller execution</summary>
    public class ValidationFilter(IServiceProvider _serviceProvider) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var parameter in context.ActionArguments.Values)
            {
                if (parameter == null) continue;

                var parameterType = parameter.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(parameter);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        throw new ValidationException(validationResult.Errors);
                    }
                }
            }

            await next();
        }
    }
}

