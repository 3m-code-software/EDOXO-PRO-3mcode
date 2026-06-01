using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Checks;

namespace EdoxoPro.Application.Interfaces;
public interface ICheckService
{
    Task<ApiResponse<PagedResult<CheckDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<CheckDto>> GetByIdAsync(int id);
    Task<ApiResponse<CheckDto>> CreateAsync(CreateCheckDto request);
    Task<ApiResponse<CheckDto>> UpdateAsync(int id, UpdateCheckDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> UpdateStatusAsync(int id, UpdateCheckStatusDto request);
}
