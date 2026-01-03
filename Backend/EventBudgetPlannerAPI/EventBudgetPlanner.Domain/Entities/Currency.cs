using EventBudgetPlanner.Domain.Common;

namespace EventBudgetPlanner.Domain.Entities;

// Currency entity for multi-currency support
public class Currency : BaseEntity
{
    public string Code { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty; 
    public string Symbol { get; set; } = string.Empty; 
    public decimal ExchangeRate { get; set; } = 1.0m; 
    public bool IsBaseCurrency { get; set; } = false; 
    public bool IsActive { get; set; } = true;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

