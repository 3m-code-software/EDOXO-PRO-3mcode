using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class SaleReturnItemConfiguration : IEntityTypeConfiguration<SaleReturnItem>
{
    public void Configure(EntityTypeBuilder<SaleReturnItem> builder)
    {
        builder.ToTable("SaleReturnItems");
        builder.HasKey(sri => sri.Id);

        builder.Property(sri => sri.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(sri => sri.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(sri => sri.Return)
            .WithMany(r => r.Items)
            .HasForeignKey(sri => sri.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sri => sri.SaleItem)
            .WithMany()
            .HasForeignKey(sri => sri.SaleItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
