namespace EventBudgetPlanner.Domain.Common
{
    /// <summary>Base class for all entities with common properties (Id, audit fields)</summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}

