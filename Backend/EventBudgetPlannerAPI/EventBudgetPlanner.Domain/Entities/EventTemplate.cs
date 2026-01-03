using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities;

// Event template for quick event creation
public class EventTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; 
    public decimal DefaultBudget { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsPublic { get; set; } = false; 
    public string CreatedBy { get; set; } = string.Empty; 
    public List<EventTemplateCategory> DefaultCategories { get; set; } = new();
}

// Default expense categories for event templates
public class EventTemplateCategory : BaseEntity
{
    public int EventTemplateId { get; set; }
    public EventTemplate EventTemplate { get; set; } = null!;
    public string CategoryName { get; set; } = string.Empty; 
    public decimal EstimatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
}

