using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.DiscountType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Subtotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Discount)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Tax)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.TaxRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Total)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.PaidAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.ShippingStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Delegate)
            .WithMany(d => d.Sales)
            .HasForeignKey(s => s.DelegateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.InvoiceNumber).IsUnique();
        builder.HasIndex(s => s.Date);
    }
}
