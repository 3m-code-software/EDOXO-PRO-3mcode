using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;

namespace EdoxoPro.Application.Interfaces;
public interface IStockTransferService
{
    Task<ApiResponse<PagedResult<StockTransferDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<StockTransferDto>> GetByIdAsync(int id);
    Task<ApiResponse<StockTransferDto>> CreateAsync(CreateStockTransferDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ConfirmAsync(int id);
}
