using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventBudgetPlanner.Infrastructure.Data.Configurations
{
    // Entity configuration for Expense entity
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses", t => 
            {
                t.HasCheckConstraint("CK_Expense_Amount", "Amount >= 0");
                t.HasCheckConstraint("CK_Expense_BaseAmount", "BaseAmount >= 0");
            });
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
            builder.Property(e => e.ExchangeRate).HasColumnType("decimal(18,6)");
            builder.Property(e => e.BaseAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.IsPaid).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.Date).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.Vendor).HasMaxLength(200);
            builder.Property(e => e.ReceiptImagePath).HasMaxLength(500);
            builder.Property(e => e.ReceiptFileName).HasMaxLength(255);
            builder.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.ModifiedDate);
            
            // Relationships
            builder.HasOne(e => e.Event)
                .WithMany(ev => ev.Expenses)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(e => e.EventId);
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.IsPaid);
            builder.HasIndex(e => e.CurrencyCode);
            builder.HasIndex(e => e.Vendor);
            builder.HasIndex(e => e.Date);
        }
    }
}



