using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities;

/// <summary>Planned budget allocation for a specific category within an event.</summary>
public class EventCategoryBudget : BaseEntity
{
    public int EventId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
}










