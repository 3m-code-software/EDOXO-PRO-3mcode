using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;

namespace EdoxoPro.Application.Interfaces;
public interface IProductUnitService
{
    Task<ApiResponse<IEnumerable<ProductUnitDto>>> GetAllAsync();
    Task<ApiResponse<ProductUnitDto>> GetByIdAsync(int id);
    Task<ApiResponse<ProductUnitDto>> CreateAsync(CreateProductUnitDto request);
    Task<ApiResponse<ProductUnitDto>> UpdateAsync(int id, UpdateProductUnitDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
