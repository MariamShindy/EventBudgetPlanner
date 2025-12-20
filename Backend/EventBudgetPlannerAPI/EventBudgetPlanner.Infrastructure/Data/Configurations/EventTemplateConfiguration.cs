using EventBudgetPlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventBudgetPlanner.Infrastructure.Data.Configurations;

/// <summary>Entity configuration for EventTemplate</summary>
public class EventTemplateConfiguration : IEntityTypeConfiguration<EventTemplate>
{
    public void Configure(EntityTypeBuilder<EventTemplate> builder)
    {
        builder.HasKey(et => et.Id);

        builder.Property(et => et.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(et => et.Description)
            .HasMaxLength(1000);

        builder.Property(et => et.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(et => et.DefaultBudget)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(et => et.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(et => et.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(et => et.CreatedBy)
            .IsRequired()
            .HasMaxLength(450); // Standard length for user IDs

        // Relationships
        builder.HasMany(et => et.DefaultCategories)
            .WithOne(etc => etc.EventTemplate)
            .HasForeignKey(etc => etc.EventTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(et => et.CreatedBy);
        builder.HasIndex(et => et.IsPublic);
        builder.HasIndex(et => et.Category);

        // Constraints
        builder.ToTable("EventTemplates", t => t.HasCheckConstraint("CK_EventTemplate_DefaultBudget", "DefaultBudget >= 0"));
    }
}

