using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;

namespace EdoxoPro.Application.Interfaces;
public interface ICompanySettingService
{
    Task<ApiResponse<CompanySettingDto>> GetAsync();
    Task<ApiResponse<CompanySettingDto>> UpdateAsync(UpdateCompanySettingDto request);
}
