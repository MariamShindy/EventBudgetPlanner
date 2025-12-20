using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities
{
    /// <summary>Expense entity representing a cost item linked to an event</summary>
    public class Expense : BaseEntity
    {
        public int EventId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public decimal? ExchangeRate { get; set; } // Exchange rate at time of expense
        public decimal BaseAmount { get; set; } // Amount in base currency
        public bool IsPaid { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Vendor { get; set; }
        public string? ReceiptImagePath { get; set; }
        public string? ReceiptFileName { get; set; }
        public virtual Event Event { get; set; } = null!;
    }
}

