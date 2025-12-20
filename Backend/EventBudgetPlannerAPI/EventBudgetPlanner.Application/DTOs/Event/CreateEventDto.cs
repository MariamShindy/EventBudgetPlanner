namespace EventBudgetPlanner.Application.DTOs
{
    /// <summary>DTO for creating a new event</summary>
    public record CreateEventDto(
        string Name, 
        DateTime Date, 
        decimal Budget, 
        string? Description,
        string CurrencyCode = "USD",
        int? EventTemplateId = null);
}

