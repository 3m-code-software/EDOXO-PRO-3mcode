using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.Subtotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Tax)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.TaxRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Total)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PaidAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ReferenceNumber).IsUnique();
        builder.HasIndex(p => p.Date);
    }
}
