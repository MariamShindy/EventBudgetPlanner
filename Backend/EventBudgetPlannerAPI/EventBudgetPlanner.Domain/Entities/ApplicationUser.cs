namespace EventBudgetPlanner.Domain.Entities
{
    /// <summary>Application user entity extending IdentityUser with custom properties</summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}

