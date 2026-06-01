using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Users;

namespace EdoxoPro.Application.Interfaces;
public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<UserDto>> GetByIdAsync(int id);
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto request);
    Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ActivateAsync(int id);
}
