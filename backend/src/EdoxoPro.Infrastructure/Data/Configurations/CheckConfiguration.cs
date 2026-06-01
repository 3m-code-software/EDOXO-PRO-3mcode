using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class CheckConfiguration : IEntityTypeConfiguration<Check>
{
    public void Configure(EntityTypeBuilder<Check> builder)
    {
        builder.ToTable("Checks");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CheckNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.BankName)
            .HasMaxLength(200);

        builder.Property(c => c.Type)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.ReferenceType)
            .HasMaxLength(50);

        builder.Property(c => c.Notes)
            .HasMaxLength(500);

        builder.Property(c => c.OwnerName)
            .HasMaxLength(200);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(c => c.CheckNumber);
        builder.HasIndex(c => c.DueDate);
    }
}
