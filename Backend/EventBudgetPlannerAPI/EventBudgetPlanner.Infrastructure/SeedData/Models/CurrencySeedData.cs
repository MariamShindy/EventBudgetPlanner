using System.Text.Json.Serialization;

namespace EventBudgetPlanner.Infrastructure.SeedData.Models;

/// <summary>Currency seed data model</summary>
public class CurrencySeedData
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    
    [JsonPropertyName("exchangeRate")]
    public decimal ExchangeRate { get; set; }
    
    [JsonPropertyName("isBaseCurrency")]
    public bool IsBaseCurrency { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
