using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;

namespace EdoxoPro.Application.Interfaces;
public interface IBarcodeSettingService
{
    Task<ApiResponse<BarcodeSettingDto>> GetAsync();
    Task<ApiResponse<BarcodeSettingDto>> UpdateAsync(UpdateBarcodeSettingDto request);
}
