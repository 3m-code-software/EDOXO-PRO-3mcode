using Microsoft.EntityFrameworkCore;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ModelBuilderExtensions)
                    .GetMethod(nameof(SetIsDeletedFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, [modelBuilder]);
            }
        }
    }

    private static void SetIsDeletedFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, Name = "Electronics", IsActive = true },
            new ProductCategory { Id = 2, Name = "Clothing", IsActive = true },
            new ProductCategory { Id = 3, Name = "Food & Beverages", IsActive = true },
            new ProductCategory { Id = 4, Name = "Office Supplies", IsActive = true },
            new ProductCategory { Id = 5, Name = "Hardware", IsActive = true }
        );

        modelBuilder.Entity<ProductBrand>().HasData(
            new ProductBrand { Id = 1, Name = "Samsung", IsActive = true },
            new ProductBrand { Id = 2, Name = "Nike", IsActive = true },
            new ProductBrand { Id = 3, Name = "Local Brand", IsActive = true }
        );

        modelBuilder.Entity<ProductUnit>().HasData(
            new ProductUnit { Id = 1, Name = "Piece", ShortName = "pc", IsActive = true },
            new ProductUnit { Id = 2, Name = "Kilogram", ShortName = "kg", IsActive = true },
            new ProductUnit { Id = 3, Name = "Liter", ShortName = "L", IsActive = true }
        );
    }
}
