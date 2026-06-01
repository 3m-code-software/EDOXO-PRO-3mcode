using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;

namespace EdoxoPro.Application.Interfaces;
public interface IPurchaseReturnService
{
    Task<ApiResponse<PagedResult<PurchaseReturnDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<PurchaseReturnDto>> GetByIdAsync(int id);
    Task<ApiResponse<PurchaseReturnDto>> CreateAsync(CreatePurchaseReturnDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
