using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities;

/// <summary>Event template for quick event creation</summary>
public class EventTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty; // "Wedding", "Birthday Party", "Corporate Event"
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Personal", "Corporate", "Social"
    public decimal DefaultBudget { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsPublic { get; set; } = false; // Can be shared with other users
    public string CreatedBy { get; set; } = string.Empty; // User ID who created the template
    public List<EventTemplateCategory> DefaultCategories { get; set; } = new();
}

/// <summary>Default expense categories for event templates</summary>
public class EventTemplateCategory : BaseEntity
{
    public int EventTemplateId { get; set; }
    public EventTemplate EventTemplate { get; set; } = null!;
    public string CategoryName { get; set; } = string.Empty; // "Food", "Venue", "Decorations"
    public decimal EstimatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
}

