using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class DamagedStockItemConfiguration : IEntityTypeConfiguration<DamagedStockItem>
{
    public void Configure(EntityTypeBuilder<DamagedStockItem> builder)
    {
        builder.ToTable("DamagedStockItems");
        builder.HasKey(dsi => dsi.Id);

        builder.Property(dsi => dsi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(dsi => dsi.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(dsi => dsi.DamagedStock)
            .WithMany(ds => ds.Items)
            .HasForeignKey(dsi => dsi.DamagedStockId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dsi => dsi.Product)
            .WithMany()
            .HasForeignKey(dsi => dsi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
