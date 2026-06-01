using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;

namespace EdoxoPro.Application.Interfaces;
public interface IProductService
{
    Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductFilterRequest request);
    Task<ApiResponse<ProductDto>> GetByIdAsync(int id);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto request);
    Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<ProductDto>> GetByBarcodeAsync(string barcode);
    Task<ApiResponse<string>> UpdateStockAsync(int id, double quantity);
}
