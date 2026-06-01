using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.ToTable("StockTransfers");
        builder.HasKey(st => st.Id);

        builder.Property(st => st.TransferNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(st => st.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(st => st.Notes)
            .HasMaxLength(500);

        builder.HasOne(st => st.FromWarehouse)
            .WithMany(w => w.StockTransfersFrom)
            .HasForeignKey(st => st.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.ToWarehouse)
            .WithMany(w => w.StockTransfersTo)
            .HasForeignKey(st => st.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(st => st.TransferNumber).IsUnique();
    }
}
