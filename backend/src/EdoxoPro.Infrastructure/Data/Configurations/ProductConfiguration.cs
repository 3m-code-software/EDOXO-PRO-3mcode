using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.NameAr)
            .HasMaxLength(200);

        builder.Property(p => p.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CostPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.SalePrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.WholesalePrice)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Unit)
            .WithMany()
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.Name);
    }
}
