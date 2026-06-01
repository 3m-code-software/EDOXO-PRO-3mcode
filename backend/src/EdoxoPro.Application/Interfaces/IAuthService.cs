using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Auth;

namespace EdoxoPro.Application.Interfaces;
public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<string>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<ApiResponse<UserInfo>> GetProfileAsync(int userId);
    Task<ApiResponse<UserInfo>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
