using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;

namespace EdoxoPro.Application.Interfaces;
public interface ISupplierService
{
    Task<ApiResponse<PagedResult<SupplierDto>>> GetAllAsync(SupplierFilterRequest request);
    Task<ApiResponse<SupplierDto>> GetByIdAsync(int id);
    Task<ApiResponse<SupplierDto>> CreateAsync(CreateSupplierDto request);
    Task<ApiResponse<SupplierDto>> UpdateAsync(int id, UpdateSupplierDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<byte[]>> ExportAsync(FilterRequest request);
}
