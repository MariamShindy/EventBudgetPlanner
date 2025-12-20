using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventBudgetPlanner.Infrastructure.Data.Configurations
{
    /// <summary>Entity configuration for Event entity</summary>
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Date).IsRequired();
            builder.Property(e => e.Budget).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
            builder.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.ModifiedDate);
            
            // Relationships
            builder.HasOne(e => e.EventTemplate)
                .WithMany()
                .HasForeignKey(e => e.EventTemplateId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasMany(e => e.Expenses)
                .WithOne(exp => exp.Event)
                .HasForeignKey(exp => exp.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.CurrencyCode);
            builder.HasIndex(e => e.EventTemplateId);
        }
    }
}



