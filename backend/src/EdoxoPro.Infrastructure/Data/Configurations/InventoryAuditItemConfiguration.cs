using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class InventoryAuditItemConfiguration : IEntityTypeConfiguration<InventoryAuditItem>
{
    public void Configure(EntityTypeBuilder<InventoryAuditItem> builder)
    {
        builder.ToTable("InventoryAuditItems");
        builder.HasKey(iai => iai.Id);

        builder.Property(iai => iai.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(iai => iai.Audit)
            .WithMany(ia => ia.Items)
            .HasForeignKey(iai => iai.AuditId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(iai => iai.Product)
            .WithMany()
            .HasForeignKey(iai => iai.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
