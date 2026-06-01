using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.ToTable("PurchaseReturns");
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.ReturnNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pr => pr.Reason)
            .HasMaxLength(500);

        builder.Property(pr => pr.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(pr => pr.Purchase)
            .WithMany()
            .HasForeignKey(pr => pr.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pr => pr.ReturnNumber).IsUnique();
    }
}
