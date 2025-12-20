using System.Text.Json.Serialization;

namespace EventBudgetPlanner.Infrastructure.SeedData.Models
{
    /// <summary>Model for event seed data from JSON</summary>
    public class EventSeedData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("budget")]
        public decimal Budget { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "USD";
        
        [JsonPropertyName("eventTemplateId")]
        public int? EventTemplateId { get; set; }

        [JsonPropertyName("isTemplate")]
        public bool IsTemplate { get; set; } = false;
    }
}

