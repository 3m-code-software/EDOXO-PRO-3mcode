using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class InvoiceSettingConfiguration : IEntityTypeConfiguration<InvoiceSetting>
{
    public void Configure(EntityTypeBuilder<InvoiceSetting> builder)
    {
        builder.ToTable("InvoiceSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Prefix)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Footer)
            .HasMaxLength(500);

        builder.Property(s => s.TaxRate)
            .HasColumnType("decimal(18,2)");
    }
}
