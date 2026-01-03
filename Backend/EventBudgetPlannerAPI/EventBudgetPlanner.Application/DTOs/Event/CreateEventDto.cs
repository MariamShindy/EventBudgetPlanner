namespace EventBudgetPlanner.Application.DTOs
{
    //DTO for creating a new event
    public record CreateEventDto(
        string Name, 
        DateTime Date, 
        decimal Budget, 
        string? Description,
        string CurrencyCode = "USD",
        int? EventTemplateId = null);
}

