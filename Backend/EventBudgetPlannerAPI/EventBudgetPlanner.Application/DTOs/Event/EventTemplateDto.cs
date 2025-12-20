namespace EventBudgetPlanner.Application.DTOs.Event;

/// <summary>Event template data transfer object</summary>
public class EventTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal DefaultBudget { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsPublic { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<EventTemplateCategoryDto> DefaultCategories { get; set; } = new();
}

/// <summary>Event template category DTO</summary>
public class EventTemplateCategoryDto
{
    public int Id { get; set; }
    public int EventTemplateId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>Create event template DTO</summary>
public class CreateEventTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal DefaultBudget { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsPublic { get; set; } = false;
    public List<CreateEventTemplateCategoryDto> DefaultCategories { get; set; } = new();
}

/// <summary>Create event template category DTO</summary>
public class CreateEventTemplateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
}

/// <summary>Update event template DTO</summary>
public class UpdateEventTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal DefaultBudget { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsPublic { get; set; } = false;
    public List<UpdateEventTemplateCategoryDto> DefaultCategories { get; set; } = new();
}

/// <summary>Update event template category DTO</summary>
public class UpdateEventTemplateCategoryDto
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
}

/// <summary>Create event from template DTO</summary>
public class CreateEventFromTemplateDto
{
    public int EventTemplateId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string EventDescription { get; set; } = string.Empty;
    public decimal? EventBudget { get; set; }
    public string? EventCurrencyCode { get; set; }
}
