using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold);
    Task<IReadOnlyList<Product>> GetTopSellingAsync(int count);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm);
}
