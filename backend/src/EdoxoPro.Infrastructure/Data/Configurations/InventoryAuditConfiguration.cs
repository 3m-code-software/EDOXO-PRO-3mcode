using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class InventoryAuditConfiguration : IEntityTypeConfiguration<InventoryAudit>
{
    public void Configure(EntityTypeBuilder<InventoryAudit> builder)
    {
        builder.ToTable("InventoryAudits");
        builder.HasKey(ia => ia.Id);

        builder.Property(ia => ia.AuditNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ia => ia.Notes)
            .HasMaxLength(1000);

        builder.Property(ia => ia.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(ia => ia.Warehouse)
            .WithMany()
            .HasForeignKey(ia => ia.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ia => ia.AuditNumber).IsUnique();
    }
}
