using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities;

/// <summary>Currency entity for multi-currency support</summary>
public class Currency : BaseEntity
{
    public string Code { get; set; } = string.Empty; // USD, EUR, GBP, etc.
    public string Name { get; set; } = string.Empty; // US Dollar, Euro, British Pound
    public string Symbol { get; set; } = string.Empty; // $, €, £
    public decimal ExchangeRate { get; set; } = 1.0m; // Exchange rate to base currency
    public bool IsBaseCurrency { get; set; } = false; // Only one currency can be base
    public bool IsActive { get; set; } = true;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

