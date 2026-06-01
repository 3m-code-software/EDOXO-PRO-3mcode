using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;

namespace EdoxoPro.Application.Interfaces;
public interface IInvoiceSettingService
{
    Task<ApiResponse<InvoiceSettingDto>> GetAsync();
    Task<ApiResponse<InvoiceSettingDto>> UpdateAsync(UpdateInvoiceSettingDto request);
}
