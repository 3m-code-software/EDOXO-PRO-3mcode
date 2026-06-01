using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class SaleReturnConfiguration : IEntityTypeConfiguration<SaleReturn>
{
    public void Configure(EntityTypeBuilder<SaleReturn> builder)
    {
        builder.ToTable("SaleReturns");
        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.ReturnNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sr => sr.Reason)
            .HasMaxLength(500);

        builder.Property(sr => sr.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(sr => sr.Sale)
            .WithMany()
            .HasForeignKey(sr => sr.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sr => sr.ReturnNumber).IsUnique();
    }
}
