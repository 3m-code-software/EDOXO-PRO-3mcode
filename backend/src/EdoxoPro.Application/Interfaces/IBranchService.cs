using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;

namespace EdoxoPro.Application.Interfaces;
public interface IBranchService
{
    Task<ApiResponse<IEnumerable<BranchDto>>> GetAllAsync();
    Task<ApiResponse<BranchDto>> GetByIdAsync(int id);
    Task<ApiResponse<BranchDto>> CreateAsync(CreateBranchDto request);
    Task<ApiResponse<BranchDto>> UpdateAsync(int id, UpdateBranchDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
