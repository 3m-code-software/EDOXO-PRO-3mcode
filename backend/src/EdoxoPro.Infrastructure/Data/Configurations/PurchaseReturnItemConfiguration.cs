using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> builder)
    {
        builder.ToTable("PurchaseReturnItems");
        builder.HasKey(pri => pri.Id);

        builder.Property(pri => pri.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(pri => pri.Return)
            .WithMany(r => r.Items)
            .HasForeignKey(pri => pri.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pri => pri.PurchaseItem)
            .WithMany()
            .HasForeignKey(pri => pri.PurchaseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
