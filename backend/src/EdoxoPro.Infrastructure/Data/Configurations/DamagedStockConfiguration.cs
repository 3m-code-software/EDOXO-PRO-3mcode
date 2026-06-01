using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class DamagedStockConfiguration : IEntityTypeConfiguration<DamagedStock>
{
    public void Configure(EntityTypeBuilder<DamagedStock> builder)
    {
        builder.ToTable("DamagedStocks");
        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.ReferenceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ds => ds.Reason)
            .HasMaxLength(500);

        builder.Property(ds => ds.Notes)
            .HasMaxLength(1000);

        builder.Property(ds => ds.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(ds => ds.Warehouse)
            .WithMany()
            .HasForeignKey(ds => ds.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ds => ds.ReferenceNumber).IsUnique();
    }
}
