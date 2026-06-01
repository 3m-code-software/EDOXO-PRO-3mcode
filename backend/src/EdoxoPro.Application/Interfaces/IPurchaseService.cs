using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;

namespace EdoxoPro.Application.Interfaces;
public interface IPurchaseService
{
    Task<ApiResponse<PagedResult<PurchaseDto>>> GetAllAsync(PurchaseFilterRequest request);
    Task<ApiResponse<PurchaseDto>> GetByIdAsync(int id);
    Task<ApiResponse<PurchaseDto>> CreateAsync(CreatePurchaseDto request);
    Task<ApiResponse<PurchaseDto>> UpdateAsync(int id, UpdatePurchaseDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ReceiveAsync(int id);
}
