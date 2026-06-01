using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.AttributeName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pv => pv.AttributeValue)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pv => pv.Barcode)
            .HasMaxLength(100);

        builder.Property(pv => pv.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(pv => pv.Cost)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
