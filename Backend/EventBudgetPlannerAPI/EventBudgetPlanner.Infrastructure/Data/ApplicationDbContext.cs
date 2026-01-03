using EventBudgetPlanner.Infrastructure.Data.Configurations;

namespace EventBudgetPlanner.Infrastructure.Data
{
    // Database context for Event Budget Planner with Identity suppor
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<EventTemplate> EventTemplates { get; set; }
        public DbSet<EventTemplateCategory> EventTemplateCategories { get; set; }
        public DbSet<EventCategoryBudget> EventCategoryBudgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new EventTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new EventTemplateCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EventCategoryBudgetConfiguration());
        }
    }
}

