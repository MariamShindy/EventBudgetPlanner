using System.Text.Json.Serialization;

namespace EventBudgetPlanner.Infrastructure.SeedData.Models
{
    // Model for user seed data from JSON
    public class UserSeedData
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
    }
}

