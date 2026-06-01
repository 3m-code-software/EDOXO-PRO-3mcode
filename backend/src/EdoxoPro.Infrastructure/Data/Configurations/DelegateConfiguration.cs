using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class DelegateConfiguration : IEntityTypeConfiguration<Domain.Entities.Delegate>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Delegate> builder)
    {
        builder.ToTable("Delegates");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .HasMaxLength(50);

        builder.Property(d => d.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Email)
            .HasMaxLength(200);

        builder.Property(d => d.Phone)
            .HasMaxLength(20);

        builder.Property(d => d.Address)
            .HasMaxLength(500);

        builder.Property(d => d.CommissionPercent)
            .HasColumnType("decimal(18,2)");
    }
}
