using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;

namespace EdoxoPro.Application.Interfaces;
public interface IInventoryAuditService
{
    Task<ApiResponse<PagedResult<InventoryAuditDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<InventoryAuditDto>> GetByIdAsync(int id);
    Task<ApiResponse<InventoryAuditDto>> CreateAsync(CreateInventoryAuditDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> StartAsync(int id);
    Task<ApiResponse<string>> CompleteAsync(int id);
}
