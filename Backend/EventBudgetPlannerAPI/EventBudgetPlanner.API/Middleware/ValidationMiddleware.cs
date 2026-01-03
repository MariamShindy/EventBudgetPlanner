using FluentValidation;
using System.Net;
using System.Text.Json;

namespace EventBudgetPlanner.API.Middleware
{
    //Validation middleware for handling FluentValidation exceptions
    public class ValidationMiddleware(RequestDelegate _next , ILogger<ValidationMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {ValidationErrors}",
                    string.Join("; ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                await HandleValidationExceptionAsync(context, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var errorResponse = new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path.Value
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }
    }

    //Extension methods for ValidationMiddleware registration
    public static class ValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidation(this IApplicationBuilder app) => app.UseMiddleware<ValidationMiddleware>();
    }
}

