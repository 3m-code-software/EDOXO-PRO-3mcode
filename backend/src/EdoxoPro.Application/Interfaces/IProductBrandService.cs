using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;

namespace EdoxoPro.Application.Interfaces;
public interface IProductBrandService
{
    Task<ApiResponse<IEnumerable<ProductBrandDto>>> GetAllAsync();
    Task<ApiResponse<ProductBrandDto>> GetByIdAsync(int id);
    Task<ApiResponse<ProductBrandDto>> CreateAsync(CreateProductBrandDto request);
    Task<ApiResponse<ProductBrandDto>> UpdateAsync(int id, UpdateProductBrandDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
