using Microsoft.EntityFrameworkCore;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Infrastructure.Data;

namespace EdoxoPro.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Barcode == barcode);
    }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Unit)
            .Where(p => p.CurrentStock <= threshold)
            .OrderBy(p => p.CurrentStock)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetTopSellingAsync(int count)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Unit)
            .OrderByDescending(p => p.SaleItems.Sum(si => si.Quantity))
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.Trim().ToLower();
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Unit)
            .Where(p => p.Name.ToLower().Contains(term) ||
                        (p.NameAr != null && p.NameAr.Contains(term)) ||
                        p.SKU.ToLower().Contains(term) ||
                        (p.Barcode != null && p.Barcode.Contains(term)))
            .ToListAsync();
    }
}
