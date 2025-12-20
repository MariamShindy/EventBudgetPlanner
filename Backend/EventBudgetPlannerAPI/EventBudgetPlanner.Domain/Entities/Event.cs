using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities
{
    /// <summary>Event entity representing a budget-tracked event with associated expenses</summary>
    public class Event : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Budget { get; set; }
        public string? Description { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public int? EventTemplateId { get; set; }
        public EventTemplate? EventTemplate { get; set; }
        public string? ShareToken { get; set; } // Unique token for sharing events
        public bool IsTemplate { get; set; } = false; // Indicates if this is a template event
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}

