using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;

namespace EdoxoPro.Application.Interfaces;
public interface IDamagedStockService
{
    Task<ApiResponse<PagedResult<DamagedStockDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<DamagedStockDto>> GetByIdAsync(int id);
    Task<ApiResponse<DamagedStockDto>> CreateAsync(CreateDamagedStockDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
