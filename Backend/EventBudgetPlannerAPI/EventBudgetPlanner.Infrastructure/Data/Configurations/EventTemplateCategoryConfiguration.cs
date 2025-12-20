using EventBudgetPlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventBudgetPlanner.Infrastructure.Data.Configurations;

/// <summary>Entity configuration for EventTemplateCategory</summary>
public class EventTemplateCategoryConfiguration : IEntityTypeConfiguration<EventTemplateCategory>
{
    public void Configure(EntityTypeBuilder<EventTemplateCategory> builder)
    {
        builder.HasKey(etc => etc.Id);

        builder.Property(etc => etc.CategoryName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(etc => etc.EstimatedAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(etc => etc.Description)
            .HasMaxLength(500);

        builder.Property(etc => etc.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(etc => etc.EventTemplate)
            .WithMany(et => et.DefaultCategories)
            .HasForeignKey(etc => etc.EventTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(etc => etc.EventTemplateId);
        builder.HasIndex(etc => new { etc.EventTemplateId, etc.SortOrder });

        // Constraints
        builder.ToTable("EventTemplateCategories", t => t.HasCheckConstraint("CK_EventTemplateCategory_EstimatedAmount", "EstimatedAmount >= 0"));
    }
}

