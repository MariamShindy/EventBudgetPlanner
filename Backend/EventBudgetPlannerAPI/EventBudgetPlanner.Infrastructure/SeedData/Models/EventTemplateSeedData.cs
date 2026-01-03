using System.Text.Json.Serialization;

namespace EventBudgetPlanner.Infrastructure.SeedData.Models;

// Event template seed data model
public class EventTemplateSeedData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    
    [JsonPropertyName("defaultBudget")]
    public decimal DefaultBudget { get; set; }
    
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = "USD";
    
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }
    
    [JsonPropertyName("defaultCategories")]
    public List<EventTemplateCategorySeedData> DefaultCategories { get; set; } = new();
}

/// <summary>Event template category seed data model</summary>
public class EventTemplateCategorySeedData
{
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;
    
    [JsonPropertyName("estimatedAmount")]
    public decimal EstimatedAmount { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }
}
