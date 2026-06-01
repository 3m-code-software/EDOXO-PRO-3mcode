using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;

namespace EdoxoPro.Application.Interfaces;
public interface ISaleService
{
    Task<ApiResponse<PagedResult<SaleDto>>> GetAllAsync(SaleFilterRequest request);
    Task<ApiResponse<SaleDto>> GetByIdAsync(int id);
    Task<ApiResponse<SaleDto>> CreateAsync(CreateSaleDto request);
    Task<ApiResponse<SaleDto>> UpdateAsync(int id, UpdateSaleDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ApproveAsync(int id);
    Task<ApiResponse<string>> PayAsync(int id, PaySaleDto request);
    Task<ApiResponse<IEnumerable<SaleDto>>> GetDraftsAsync();
    Task<ApiResponse<IEnumerable<SaleDto>>> GetQuotesAsync();
    Task<ApiResponse<byte[]>> ExportAsync(SaleFilterRequest request);
}
