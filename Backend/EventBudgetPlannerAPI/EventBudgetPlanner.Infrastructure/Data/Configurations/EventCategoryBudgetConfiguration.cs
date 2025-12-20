using EventBudgetPlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventBudgetPlanner.Infrastructure.Data.Configurations;

/// <summary>Entity configuration for EventCategoryBudget</summary>
public class EventCategoryBudgetConfiguration : IEntityTypeConfiguration<EventCategoryBudget>
{
    public void Configure(EntityTypeBuilder<EventCategoryBudget> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PlannedAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(e => new { e.EventId, e.Category })
            .IsUnique();
    }
}










