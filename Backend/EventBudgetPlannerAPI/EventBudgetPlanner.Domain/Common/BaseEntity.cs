namespace EventBudgetPlanner.Domain.Common
{
    // Base class for all entities with common properties
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}

