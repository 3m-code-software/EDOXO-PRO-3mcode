using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Roles;

namespace EdoxoPro.Application.Interfaces;
public interface IRoleService
{
    Task<ApiResponse<IEnumerable<RoleDto>>> GetAllAsync();
    Task<ApiResponse<RoleDto>> GetByIdAsync(int id);
    Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto request);
    Task<ApiResponse<RoleDto>> UpdateAsync(int id, UpdateRoleDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
