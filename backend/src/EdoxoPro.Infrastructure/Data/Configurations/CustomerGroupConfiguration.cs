using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Data.Configurations;

public class CustomerGroupConfiguration : IEntityTypeConfiguration<CustomerGroup>
{
    public void Configure(EntityTypeBuilder<CustomerGroup> builder)
    {
        builder.ToTable("CustomerGroups");
        builder.HasKey(cg => cg.Id);

        builder.Property(cg => cg.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cg => cg.Description)
            .HasMaxLength(500);

        builder.Property(cg => cg.DiscountPercent)
            .HasColumnType("decimal(18,2)");
    }
}
