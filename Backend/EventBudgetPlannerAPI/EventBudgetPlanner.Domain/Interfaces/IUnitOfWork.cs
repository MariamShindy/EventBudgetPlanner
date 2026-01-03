using EventBudgetPlanner.Domain.Entities;

namespace EventBudgetPlanner.Domain.Interfaces
{
    // Unit of Work interface coordinating repositories and managing transactions
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Event> Events { get; }
        IRepository<Expense> Expenses { get; }
        IRepository<EventTemplateCategory> EventTemplateCategories { get; }
        IRepository<EventCategoryBudget> EventCategoryBudgets { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

