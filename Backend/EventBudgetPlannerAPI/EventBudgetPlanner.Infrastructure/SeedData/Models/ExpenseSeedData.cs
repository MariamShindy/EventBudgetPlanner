using System.Text.Json.Serialization;

namespace EventBudgetPlanner.Infrastructure.SeedData.Models
{
    /// <summary>Model for expense seed data from JSON</summary>
    public class ExpenseSeedData
    {
        [JsonPropertyName("eventIndex")]
        public int EventIndex { get; set; }
        
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "USD";
        
        [JsonPropertyName("exchangeRate")]
        public decimal? ExchangeRate { get; set; }
        
        [JsonPropertyName("baseAmount")]
        public decimal? BaseAmount { get; set; }
        
        [JsonPropertyName("isPaid")]
        public bool IsPaid { get; set; }
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }
        
        [JsonPropertyName("receiptImagePath")]
        public string? ReceiptImagePath { get; set; }
        
        [JsonPropertyName("receiptFileName")]
        public string? ReceiptFileName { get; set; }
        
        // Legacy property for backward compatibility
        [JsonPropertyName("paid")]
        public bool Paid 
        { 
            get => IsPaid; 
            set => IsPaid = value; 
        }
    }
}

