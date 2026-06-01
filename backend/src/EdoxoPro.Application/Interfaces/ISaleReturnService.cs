using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;

namespace EdoxoPro.Application.Interfaces;
public interface ISaleReturnService
{
    Task<ApiResponse<PagedResult<SaleReturnDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<SaleReturnDto>> GetByIdAsync(int id);
    Task<ApiResponse<SaleReturnDto>> CreateAsync(CreateSaleReturnDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
