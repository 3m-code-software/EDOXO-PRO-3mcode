using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;

namespace EdoxoPro.Application.Interfaces;
public interface IDelegateService
{
    Task<ApiResponse<PagedResult<DelegateDto>>> GetAllAsync(DelegateFilterRequest request);
    Task<ApiResponse<DelegateDto>> GetByIdAsync(int id);
    Task<ApiResponse<DelegateDto>> CreateAsync(CreateDelegateDto request);
    Task<ApiResponse<DelegateDto>> UpdateAsync(int id, UpdateDelegateDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
