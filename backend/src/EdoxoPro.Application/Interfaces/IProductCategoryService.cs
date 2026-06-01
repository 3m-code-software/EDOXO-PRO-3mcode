using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;

namespace EdoxoPro.Application.Interfaces;
public interface IProductCategoryService
{
    Task<ApiResponse<IEnumerable<ProductCategoryDto>>> GetAllAsync();
    Task<ApiResponse<ProductCategoryDto>> GetByIdAsync(int id);
    Task<ApiResponse<ProductCategoryDto>> CreateAsync(CreateProductCategoryDto request);
    Task<ApiResponse<ProductCategoryDto>> UpdateAsync(int id, UpdateProductCategoryDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
