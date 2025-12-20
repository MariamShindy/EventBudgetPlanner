using Microsoft.EntityFrameworkCore.Storage;

namespace EventBudgetPlanner.Infrastructure.Repositories
{
    /// <summary>Unit of Work implementation with lazy-loaded repositories for single instantiation per request</summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Lazy<IRepository<Event>> _eventsRepository;
        private readonly Lazy<IRepository<Expense>> _expensesRepository;
        private readonly Lazy<IRepository<EventTemplateCategory>> _eventTemplateCategoriesRepository;
        private readonly Lazy<IRepository<EventCategoryBudget>> _eventCategoryBudgetsRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _eventsRepository = new Lazy<IRepository<Event>>(() => new Repository<Event>(_context));
            _expensesRepository = new Lazy<IRepository<Expense>>(() => new Repository<Expense>(_context));
            _eventTemplateCategoriesRepository = new Lazy<IRepository<EventTemplateCategory>>(() => new Repository<EventTemplateCategory>(_context));
            _eventCategoryBudgetsRepository = new Lazy<IRepository<EventCategoryBudget>>(() => new Repository<EventCategoryBudget>(_context));
        }

        public IRepository<Event> Events => _eventsRepository.Value;
        public IRepository<Expense> Expenses => _expensesRepository.Value;
        public IRepository<EventTemplateCategory> EventTemplateCategories => _eventTemplateCategoriesRepository.Value;
        public IRepository<EventCategoryBudget> EventCategoryBudgets => _eventCategoryBudgetsRepository.Value;

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null) await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

