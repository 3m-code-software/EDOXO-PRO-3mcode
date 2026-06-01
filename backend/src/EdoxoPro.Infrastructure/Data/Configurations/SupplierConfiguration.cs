using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ContactId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.BusinessName)
            .HasMaxLength(200);

        builder.Property(s => s.Email)
            .HasMaxLength(200);

        builder.Property(s => s.Phone)
            .HasMaxLength(20);

        builder.Property(s => s.TaxNumber)
            .HasMaxLength(50);

        builder.Property(s => s.CommercialRegister)
            .HasMaxLength(50);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.Country)
            .HasMaxLength(100);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.OpeningBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.PreviousBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.CreditLimit)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.TaxNumber);
        builder.HasIndex(s => s.ContactId).IsUnique();
    }
}
